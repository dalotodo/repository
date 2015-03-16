using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Repository.EntityFramework
{
    static class DbContexTransactionalExtensions
    {
        public static void RunWithValidateOnSaveDisabled(this DbContext db, Action<DbContext> method)
        {
            // Temporarily disable validation (already done)
            var oldValidationConfig = db.Configuration.ValidateOnSaveEnabled;

            try
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                if (method != null) method(db);
            }
            catch
            {
                throw;
            }
            finally
            {
                db.Configuration.ValidateOnSaveEnabled = oldValidationConfig;
            }
        }
    }

    public class DbContextRepositoryTransactionManager : IRepositoryTransactionManager, IDbContextFactory
    {


        private readonly DbContextRepositoryTransactionManagerOptions options = new DbContextRepositoryTransactionManagerOptions();
        private readonly List<DbContext> contexts = new List<DbContext>();
        private readonly List<DbContextTransaction> transactions = new List<DbContextTransaction>();
        private readonly Func<Type, DbContext> factoryMethod;

        private bool hasBegun = false;

        public DbContextRepositoryTransactionManager(): this(type=>(DbContext)Activator.CreateInstance(type))
        {
        }

        public DbContextRepositoryTransactionManager(Func<Type,DbContext> factory)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            this.factoryMethod = factory;
        }

        public DbContext CreateContext(Type type)
        {
            if (!typeof(DbContext).IsAssignableFrom(type)) throw new InvalidOperationException("Requested type does not inherit from DbContext.");
            var db = factoryMethod(type);
            contexts.Add(db);
            if (hasBegun) BeginContextTransaction(db);
            return db;
        }

        public DbContextRepositoryTransactionManagerOptions Options { get { return options; } }

        public void Begin()
        {
            if (hasBegun) throw new InvalidOperationException("Transaction already begun.");

            // Does nothing
            contexts.ForEach(db => BeginContextTransaction(db));

            hasBegun = true;
        }



        public void Commit()
        {
            if (!hasBegun) throw new InvalidOperationException("Cannot commit a transaction that has not begun.");

            ValidateBeforeCommit();

            // TODO: Enable fast mode to commit when saved (current algorithm has O(2n) complexity)
            contexts.ForEach(db =>
            {
                db.RunWithValidateOnSaveDisabled(c => c.SaveChanges());
            });


            transactions.ForEach(t => t.Commit());

            DisposeTransactions();
            hasBegun = false;
        }

        private void BeginContextTransaction(DbContext db)
        {
            var t = db.Database.BeginTransaction();
            transactions.Add(t);
        }

        private void ValidateBeforeCommit()
        {
            // Validate errors in all available contexts to minimize
            // chances of SaveChanges() errors
            var validationResults = contexts.Select(db =>
                new
                {
                    Context = db,
                    Errors = db.GetValidationErrors()
                });

            var contextsWithErrors = validationResults.Where(r => r.Errors.Any());

            if (contextsWithErrors.Any())
            {
                throw new AggregateException("There are validation errors in the backing contexts. See inner exceptions for details",
                    contextsWithErrors.SelectMany(db => db.Errors.Select(e => new InvalidOperationException(String.Join("\n", e.ValidationErrors))))
                    );
            }
        }

        public void Rollback()
        {
            if (!hasBegun) throw new InvalidOperationException("Cannot commit a transaction that has not begun.");

            DisposeTransactions();
            hasBegun = false;
        }

        private void DisposeTransactions()
        {
            transactions.ForEach(t => t.Dispose());
            transactions.Clear();
        }

        public void Dispose()
        {
            if (hasBegun) throw new InvalidOperationException("Transaction has begun and has not been committed nor rollbacked.");
            DisposeTransactions();
        }

    }
}

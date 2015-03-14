using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFBoost.EntityFramework
{
    /// <summary>
    /// Implements a repository service over a given Entity Framework context
    /// </summary>
    /// <typeparam name="TContext">Type of the EF DbContext</typeparam>
    /// <typeparam name="TModel">Type of the repository model</typeparam>
    public class DbContextRepositoryService<TContext,TModel> : IRepositoryService<TModel>, IDisposable
        where TContext : DbContext
        where TModel : class
    {
        private readonly TContext db;
        private readonly DbContextRepositoryOptions options;

        public DbContextRepositoryService(TContext db): this(db,DbContextRepositoryOptions.Default)
        {

        }

        public DbContextRepositoryService(TContext db, DbContextRepositoryOptions options)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;

            if (options == null) throw new ArgumentNullException("options");
            this.options = options;
        }

        /// <summary>
        /// Adds a collection of items into the DB context backed repository
        /// </summary>
        /// <param name="items">Collection of items to add</param>
        public virtual void Add(IEnumerable<TModel> items)
        {
            if (items == null) throw new ArgumentNullException("items");

            RunInDisabledChangeTrackingMode(() =>
            {
                var set = db.Set<TModel>();
                foreach (var item in items)
                {
                    set.Add(item);
                }
            });
            
            
        }

        public void Update(IEnumerable<TModel> items)
        {
            if (items == null) throw new ArgumentNullException("items");
            RunInDisabledChangeTrackingMode(() =>
            {
                foreach (var item in items)
                {
                    db.Entry(item).State = EntityState.Modified;
                }
            });
            
        }

        public void Delete(IEnumerable<TModel> items)
        {
            if (items == null) throw new ArgumentNullException("items");
            RunInDisabledChangeTrackingMode(() =>
            {
                var set = db.Set<TModel>();

                foreach (var item in items)
                {
                    set.Remove(item);
                }
            });
        }

        /// <summary>
        /// Performs a query into the DB Context backed repository
        /// </summary>
        /// <remarks>To improve performance it is a best practice not to mix in-memory items and DB backed items</remarks>
        /// <returns>A queryable collection of TModel items</returns>
        public IQueryable<TModel> Query()
        {
            var set = db.Set<TModel>();            
            
            var added = db.ChangeTracker.Entries<TModel>().Where(x => x.State == EntityState.Added).Select(x=>x.Entity);
            var hasAdded = added.Any();

            var removed = db.ChangeTracker.Entries<TModel>().Where(x => x.State == EntityState.Deleted).Select(x => x.Entity);
            var hasRemoved = removed.Any();

            // If there are no items in memory return the set (improves performance)
            if ((!hasRemoved) && (!hasAdded)) return set;
            
            // Added items + stored items but removed items (it may degrade performance)
            return added.Union(set).Except(removed).AsQueryable<TModel>();
        }

        
        private void RunInDisabledChangeTrackingMode(Action action)
        {            
            var currentADCE = db.Configuration.AutoDetectChangesEnabled;

            try
            {
                // Disable change tracking to optimize bulk item insertion
                // See: https://msdn.microsoft.com/en-us/data/jj556205.aspx
                // See also: http://blog.oneunicorn.com/2012/03/12/secrets-of-detectchanges-part-3-switching-off-automatic-detectchanges/
                db.Configuration.AutoDetectChangesEnabled = false;
                action();
            }
            finally
            {
                // And then reset change tracking back to its defaults
                db.Configuration.AutoDetectChangesEnabled = currentADCE;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Do nothing
        }
    }
}

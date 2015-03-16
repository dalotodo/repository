using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Repository.EntityFramework
{


    public class DbContextRepositoryTransactionManager : IRepositoryTransactionManager, IDbContextFactory
    {
        private readonly List<DbContext> contexts = new List<DbContext>();

        public TContext CreateContext<TContext>() where TContext : DbContext
        {
            throw new NotImplementedException();
        }

        public bool UseTransactionScope { get; set; }

        public void Begin()
        {
            // Does nothing
        }

        public void Commit()
        {            
            if (UseTransactionScope)
            {
                CommitUsingTransactionScope();
            }
            else
            {
                CommitUsingIsolatedMode();
            }
        }

        private void CommitUsingTransactionScope()
        {
            using (var t = new TransactionScope())
            {
                contexts.ForEach(db => db.SaveChanges());
                t.Complete();
            }
        }

        private void CommitUsingIsolatedMode()
        {
            throw new NotImplementedException();
        }
        
        public void Rollback()
        {
            throw new NotImplementedException();
        }
        
    }
}

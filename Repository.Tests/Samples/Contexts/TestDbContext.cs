using Repository.Tests.Samples.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Tests.Samples.Contexts
{
    class TestDbContext : DbContext
    {
        public TestDbContext(string nameOrConnectionString): base(nameOrConnectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>();
        }
    }
}

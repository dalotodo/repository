using Repository.Tests.Samples.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Tests.Samples.Contexts
{
    class HelperDbContext : DbContext
    {
        private static readonly string connectionString = @"Data Source=(LocalDB)\v11.0;Initial Catalog=RepositoryTestsHelperDb;Integrated Security=True";

        public static string ConnectionString { get { return connectionString; } }

        public HelperDbContext(): this(HelperDbContext.ConnectionString)
        {
        }

        public HelperDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>();
        }
    }
}

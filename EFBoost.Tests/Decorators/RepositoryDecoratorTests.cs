using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EFBoost.Tests.Samples.Contexts;
using EFBoost.Tests.Samples.Models;
using EFBoost.EntityFramework;

namespace EFBoost.Tests.Decorators
{
    [TestClass]
    public class RepositoryDecoratorsTests
    {
        private static readonly string connectionString = @"Data Source=(LocalDB)\v11.0;Initial Catalog=EFBoostTests;Integrated Security=True";
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            using (var db = new TestDbContext(connectionString))
            {
                db.Database.CreateIfNotExists();
            }
        }

        [TestMethod]
        public void TestAdd()
        {
            using (var db = new TestDbContext(connectionString))
            {
                using (var svc = new DbContextRepositoryService<TestDbContext,Customer>(db))
                {
                    svc.Add(
                        new Customer { Name = "A. Datum Corporation" },
                        new Customer { Name = "AdventureWorks Cycles" },
                        new Customer { Name = "Contoso Ltd." }
                        );

                    db.SaveChanges();
                    Assert.AreEqual(3, db.Set<Customer>().Count());
                }
            }
        }

        [ClassCleanup]
        public static void Cleanup()
        { 
            using (var db = new TestDbContext(connectionString))
            {
                db.Database.Delete();
            }
        }
    }
}

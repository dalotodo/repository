using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository.Tests.Samples.Contexts;
using Repository.Tests.Samples.Models;
using Repository.EntityFramework;

namespace Repository.Tests.EntityFramework
{
    [TestClass]
    public class DbContextRepositoryTests
    {
        private static readonly string connectionString = @"Data Source=(LocalDB)\v11.0;Initial Catalog=RepositoryTests;Integrated Security=True";
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            using (var db = new TestDbContext(connectionString))
            {
                db.Database.CreateIfNotExists();

                db.Set<Customer>().AddRange(
                    new Customer[]
                    {
                        new Customer { Name = "A. Datum Corporation" },
                        new Customer { Name = "AdventureWorks Cycles" },
                        new Customer { Name = "Contoso Ltd." }
                    });

                db.SaveChanges();
            }
        }

        [TestMethod]
        [TestCategory("Db Context Repository Tests")]
        [TestCategory("Repository Addition Tests")]
        public void TestAdd()
        {
            int nItems = 0;
            using (var db = new TestDbContext(connectionString))
            {
                using (var svc = db.CreateRepositoryService().OfType<Customer>())
                {
                    nItems = svc.Query().Count();
                    svc.Add(
                        new Customer { Name = "Awesome Computers" },
                        new Customer { Name = "Baldwin Museum of Science" },
                        new Customer { Name = "Blue Yonder Airlines" }
                        );

                    db.SaveChanges();
                }
            }

            using (var db = new TestDbContext(connectionString))
            {
                using (var svc = new DbContextRepositoryService<TestDbContext, Customer>(db))
                {
                    Assert.AreEqual(nItems + 3, db.Set<Customer>().Count());
                }
            }
        }



        [TestMethod]
        [TestCategory("Db Context Repository Tests")]
        [TestCategory("Repository Update Tests")]
        public void TestUpdate()
        {
            using (var db = new TestDbContext(connectionString))
            {
                using (var svc = new DbContextRepositoryService<TestDbContext, Customer>(db))
                {
                    var contoso = svc.Query().FirstOrDefault(c => c.Name.Contains("Contoso"));
                    contoso.Name = "Contoso Limited";
                    svc.Update(contoso);
                }
                db.SaveChanges();
            }

            using (var db = new TestDbContext(connectionString))
            {
                using (var svc = new DbContextRepositoryService<TestDbContext, Customer>(db))
                {
                    var contoso = svc.Query().FirstOrDefault(c => c.Name.Contains("Contoso"));
                    Assert.AreEqual("Contoso Limited", contoso.Name);
                }
            }
        }

        [TestMethod]
        [TestCategory("Db Context Repository Tests")]
        [TestCategory("Repository Deletion Tests")]
        public void TestDelete()
        {
            int id = 0;

            using (var db = new TestDbContext(connectionString))
            {
                using (var svc = new DbContextRepositoryService<TestDbContext, Customer>(db))
                {
                    var customer = svc.Query().FirstOrDefault();
                    id = customer.CustomerID;
                    svc.Delete(customer);
                }
                db.SaveChanges();
            }

            using (var db = new TestDbContext(connectionString))
            {
                using (var svc = new DbContextRepositoryService<TestDbContext, Customer>(db))
                {
                    var customer = svc.Query().FirstOrDefault(c => c.CustomerID == id);
                    Assert.IsNull(customer);
                }
            }
        }

        [TestMethod]
        [TestCategory("Db Context Repository Tests")]
        [TestCategory("Repository Query Tests")]
        [TestCategory("LINQ to Entities Query Tests")]
        public void TestQueryIncludesAddedItems()
        {
            using (var db = new TestDbContext(connectionString))
            {
                var options = new DbContextRepositoryOptions
                    {
                        IncludeAddedItemsInQuery = true,
                        ExcludeRemovedItemsInQuery = true
                    };

                using (var svc = new DbContextRepositoryService<TestDbContext, Customer>(db, options))
                {
                    var nItems = svc.Query().Count();
                    svc.Add(new Customer { Name="City Power & Light" });
                    Assert.AreEqual(nItems + 1, svc.Query().Count());
                    Assert.IsNotNull(svc.Query().FirstOrDefault(x => x.Name == "City Power & Light"));
                }
            }
        }

        [TestMethod]
        [TestCategory("Db Context Repository Tests")]
        [TestCategory("Repository Query Tests")]
        [TestCategory("LINQ to Entities Query Tests")]
        public void TestQueryDoesNotIncludeAddedItems()
        {
            using (var db = new TestDbContext(connectionString))
            {
                var options = new DbContextRepositoryOptions
                {
                    IncludeAddedItemsInQuery = false,
                    ExcludeRemovedItemsInQuery = true
                };

                using (var svc = new DbContextRepositoryService<TestDbContext, Customer>(db, options))
                {
                    var nItems = svc.Query().Count();
                    svc.Add(new Customer { Name = "City Power & Light" });
                    Assert.AreEqual(nItems, svc.Query().Count());
                    Assert.IsNull(svc.Query().FirstOrDefault(x => x.Name == "City Power & Light"));
                }
            }
        }

        [TestMethod]
        [TestCategory("Db Context Repository Tests")]
        [TestCategory("Repository Query Tests")]
        [TestCategory("LINQ to Entities Query Tests")]
        public void TestQueryExcludesDeletedItems()
        {
            using (var db = new TestDbContext(connectionString))
            {
                var options = new DbContextRepositoryOptions
                {
                    IncludeAddedItemsInQuery = true,
                    ExcludeRemovedItemsInQuery = true
                };

                using (var svc = new DbContextRepositoryService<TestDbContext, Customer>(db, options))
                {
                    var nItems = svc.Query().Count();
                    var item = svc.Query().First();
                    var id = item.CustomerID;

                    svc.Delete(item);

                    Assert.AreEqual(nItems-1, svc.Query().Count());
                    Assert.IsNull(svc.Query().FirstOrDefault(x => x.CustomerID==id));
                }
            }
        }

        [TestMethod]
        [TestCategory("Db Context Repository Tests")]
        [TestCategory("Repository Query Tests")]
        [TestCategory("LINQ to Entities Query Tests")]
        public void TestQueryDoesNotExcludeDeletedItems()
        {
            using (var db = new TestDbContext(connectionString))
            {
                var options = new DbContextRepositoryOptions
                {
                    IncludeAddedItemsInQuery = true,
                    ExcludeRemovedItemsInQuery = false
                };

                using (var svc = new DbContextRepositoryService<TestDbContext, Customer>(db, options))
                {
                    var nItems = svc.Query().Count();
                    var item = svc.Query().First();
                    var id = item.CustomerID;

                    svc.Delete(item);

                    Assert.AreEqual(nItems, svc.Query().Count());
                    Assert.IsNotNull(svc.Query().FirstOrDefault(x => x.CustomerID == id));
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

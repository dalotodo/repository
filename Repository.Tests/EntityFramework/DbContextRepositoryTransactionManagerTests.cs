using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository.EntityFramework;
using Repository.Tests.Samples.Contexts;
using Repository.Tests.Utils;
using Repository.Tests.Samples.Models;
using System.Data.Entity.Infrastructure;

namespace Repository.Tests.EntityFramework
{
    [TestClass]
    public class DbContextRepositoryTransactionManagerTests
    {

        [TestInitialize]
        public void Initialize()
        {
            using (var db = new TestDbContext())
            {
                db.DropAndCreateDatabase(d =>
                {
                    // Do nothing
                });
            }

            using (var db = new HelperDbContext())
            {
                db.DropAndCreateDatabase();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var db = new TestDbContext())
            {
                db.Database.Delete();
            }

            using (var db = new HelperDbContext())
            {
                db.Database.Delete();
            }
        }

        [TestMethod]
        [TestCategory("DB Context Transaction Manager Tests")]
        public void Begin_DoubleBegin_ShouldThrowInvalidOp()
        {
            var manager = new DbContextRepositoryTransactionManager();
            try
            {
                manager.Begin();
                manager.Begin();
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                // Success!
            }
        }

        [TestMethod]
        [TestCategory("DB Context Transaction Manager Tests")]
        public void Begin_BeginOnly_ShouldThrowInvalidOp()
        {
            
            try
            {
                using (var manager = new DbContextRepositoryTransactionManager())
                {
                    manager.Begin();
                }
                              
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                // Success!
            }
        }

        [TestMethod]
        [TestCategory("DB Context Transaction Manager Tests")]
        public void Commit_CommitWithBegin_OK()
        {
            var manager = new DbContextRepositoryTransactionManager();

            manager.Begin();
            manager.Commit();
        }

        [TestMethod]
        [TestCategory("DB Context Transaction Manager Tests")]
        public void Commit_CommitWithBeginAndRollback_ShouldThrouInvalidOp()
        {
            var manager = new DbContextRepositoryTransactionManager();

            manager.Begin();
            manager.Commit();

            try
            {
                manager.Rollback();
            }
            catch (InvalidOperationException)
            {
                // Success!
            }
        }

        [TestMethod]
        [TestCategory("DB Context Transaction Manager Tests")]
        public void Commit_CommitWithoutBegin_ShouldThrowInvalidOp()
        {
            var manager = new DbContextRepositoryTransactionManager();
            try
            {
                manager.Commit();
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                // Success!
            }
        }

        [TestMethod]
        [TestCategory("DB Context Transaction Manager Tests")]
        public void Rollback_RollbackWithBegin_OK()
        {
            var manager = new DbContextRepositoryTransactionManager();

            manager.Begin();
            manager.Rollback();
        }

        [TestMethod]
        [TestCategory("DB Context Transaction Manager Tests")]
        public void Rollback_RollbackWithBeginAndCommit_ShouldThrouInvalidOp()
        {
            var manager = new DbContextRepositoryTransactionManager();

            manager.Begin();
            manager.Rollback();

            try
            {
                manager.Commit();
            }
            catch (InvalidOperationException)
            {
                // Success!
            }
        }

        [TestMethod]
        [TestCategory("DB Context Transaction Manager Tests")]
        public void Rollback_RollbackWithoutBegin_ShouldThrowInvalidOp()
        {
            var manager = new DbContextRepositoryTransactionManager();
            try
            {
                manager.Rollback();
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                // Success!
            }
        }

        [TestMethod]        
        [TestCategory("Db Context Transaction Commit Scenarios")]
        public void Commit_SingleContext_OK()
        {
            var manager = new DbContextRepositoryTransactionManager();
            var db = manager.CreateContext<TestDbContext>();

            manager.Begin();

            db.Set<Customer>().Add(new Customer { Name = "Coho Vineyard" });

            manager.Commit();

            Assert.AreEqual(1, db.Set<Customer>().Count());
        }

        [TestMethod]      
        [TestCategory("Db Context Transaction Commit Scenarios")]
        public void Commit_MultipleContexts_OK()
        {
            using (var manager = new DbContextRepositoryTransactionManager())
            {
                var db = manager.CreateContext<TestDbContext>();
                var db2 = manager.CreateContext<HelperDbContext>();

                manager.Begin();

                db.Set<Customer>().Add(new Customer { Name = "Coho Vineyard" });
                db2.Set<Country>().Add(new Country { CountryID = "ESP", Name = "Spain" });

                manager.Commit();

                Assert.AreEqual(1, db.Set<Customer>().Count());
                Assert.AreEqual(1, db2.Set<Country>().Count());
            }
        }

        [TestMethod]
        [TestCategory("Db Context Transaction Commit Scenarios")]
        public void Commit_MultipleContextsOneFails_ShouldNotCommit()
        {
            using (var manager = new DbContextRepositoryTransactionManager())
            {
                var db = manager.CreateContext<TestDbContext>();
                var db2 = manager.CreateContext<HelperDbContext>();

                try
                {
                    manager.Begin();

                    db.Set<Customer>().Add(new Customer { Name = "Coho Vineyard" });
                    db2.Set<Country>().Add(new Country { CountryID = "ESP", Name = "Spain" });
                    db2.Set<Country>().Add(new Country { CountryID = "ESP", Name = "Spain" });
                    manager.Commit();
                } catch 
                {
                    manager.Rollback();
                }
                
                Assert.AreEqual(0, db.Set<Customer>().Count(), "Customers on first DB do not match.");
                Assert.AreEqual(0, db2.Set<Country>().Count(), "Countries on second DB do not match.");
            }
        }




    }
}

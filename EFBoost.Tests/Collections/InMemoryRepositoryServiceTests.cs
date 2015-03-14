﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using EFBoost.Collections;
using EFBoost.Tests.Samples.Models;

namespace EFBoost.Tests.Collections
{
    [TestClass]
    public class InMemoryRepositoryServiceTests
    {
        private Lazy<ICollection<Customer>> _collection = new Lazy<ICollection<Customer>>(
            () =>
            {
                var collection = new HashSet<Customer>();
                collection.Add(new Customer { CustomerID = 1, Name = "A. Datum Corporation" });
                collection.Add(new Customer { CustomerID = 2, Name = "AdventureWorks Cycles" });
                collection.Add(new Customer { CustomerID = 3, Name = "Contoso Ltd." });
                return collection;
            }
        );

        private ICollection<Customer> Collection { get { return _collection.Value; } }

        [TestMethod]
        public void TestAdd()
        {
            using (var svc = new InMemoryRepositoryService<Customer>(Collection))
            {
                var nItems = Collection.Count;

                svc.Add(
                    new Customer { CustomerID = nItems++, Name = "Alpine Ski House" },
                    new Customer { CustomerID = nItems++, Name = "Awesome Computers" },
                    new Customer { CustomerID = nItems++, Name = "Baldwin Museum of Science" }
                    );

                Assert.AreEqual(nItems, Collection.Count);
            }
        }

        [TestMethod]
        public void TestUpdate()
        {
            using (var svc = new InMemoryRepositoryService<Customer>(Collection))
            {
                var contoso = svc.Query().First(x => x.CustomerID == 3);
                contoso.Name = "Contoso Limited";
                svc.Update(contoso);
            }

            using (var svc = new InMemoryRepositoryService<Customer>(Collection))
            {
                var contoso = svc.Query().First(x => x.CustomerID == 3);
                Assert.AreEqual("Contoso Limited", contoso.Name);
            }
        }
    }
}

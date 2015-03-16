using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using Repository.Mvc;
using Moq;
using System.Collections.Generic;
using System.Web.Routing;

namespace Repository.Tests.Mvc
{

    [TestClass]
    public class TransactionalAttributeTests
    {
        class FakeController: Controller
        {
            public ActionResult Index() { return View(); }
        }

        class MockDependencyResolver: IDependencyResolver
        {
            private readonly IDependencyResolver inner;
            private readonly Mock<IDependencyResolver> mock = new Mock<IDependencyResolver>();

            public MockDependencyResolver(IDependencyResolver inner)
            {
                if (inner == null) throw new ArgumentNullException("inner");
                this.inner = inner;
            }

            public object GetService(Type serviceType)
            {
                var obj = mock.Object.GetService(serviceType);
                if (obj != null) return obj;
                return inner.GetService(serviceType);
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                var objs = mock.Object.GetServices(serviceType);
                if (objs != null) return objs;
                return inner.GetServices(serviceType);
            }

            public Mock<IDependencyResolver> Mock { get { return mock; } }
        }

        [TestInitialize]
        public void Initialize()
        {
            
        }

        [TestMethod]
        public void TestTransactionalAttributeCommit()
        {
            var mock = new Moq.Mock<IRepositoryTransactionManager>();
           
            var dResolver = new MockDependencyResolver(DependencyResolver.Current);
            dResolver.Mock.Setup(r => r.GetService(typeof(IRepositoryTransactionManager))).Returns(mock.Object);
            DependencyResolver.SetResolver(dResolver);
            

            var filter = new TransactionalAttribute();
            filter.OnActionExecuting(new ActionExecutingContext());
            filter.OnActionExecuted(new ActionExecutedContext());

            mock.Verify(t => t.Begin(), Times.Once());
            mock.Verify(t => t.Commit(), Times.Once());
            mock.Verify(t => t.Rollback(), Times.Never());

        }

        [TestMethod]
        public void TestTransactionalAttributeRollback()
        {
            var mock = new Moq.Mock<IRepositoryTransactionManager>();         

            var dResolver = new MockDependencyResolver(DependencyResolver.Current);
            dResolver.Mock.Setup(r => r.GetService(typeof(IRepositoryTransactionManager))).Returns(mock.Object);
            DependencyResolver.SetResolver(dResolver);

            var controller = new FakeController();
            var request = new Mock<RequestContext>();

            // Set up ActionExecutedContext
            var controllerDescriptor = new ReflectedControllerDescriptor(typeof(FakeController));
            var method = typeof(FakeController).GetMethod("Index");
            var actionDescriptor = new ReflectedActionDescriptor(method, "Index", controllerDescriptor);

            // Set up ActionExecutingContext
            var actionExecutingContext  = new ActionExecutingContext(
                controllerContext: new ControllerContext(requestContext: request.Object, controller: controller),
                actionDescriptor: actionDescriptor,
                actionParameters: new Dictionary<string,object>() );

            
            var actionExecutedContext = new ActionExecutedContext
                (
                controllerContext: new ControllerContext(requestContext: request.Object, controller: controller),
                actionDescriptor: actionDescriptor,
                canceled: false,
                exception : new InvalidOperationException("Test exception")
                );
            

            var filter = new TransactionalAttribute();
            filter.OnActionExecuting(actionExecutingContext);
            filter.OnActionExecuted(actionExecutedContext);

            mock.Verify(t => t.Begin(), Times.Once());
            mock.Verify(t => t.Commit(), Times.Never());
            mock.Verify(t => t.Rollback(), Times.Once());

        }
    }
}

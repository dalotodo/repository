using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Repository.Mvc
{
    /// <summary>
    /// Decorates a Controller Action to be transactional through the ASP.NET MVC Pipeline
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class TransactionalAttribute : ActionFilterAttribute
    {
        private IRepositoryTransactionManager transaction;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            transaction = DependencyResolver.Current.GetService<IRepositoryTransactionManager>();
            if (transaction == null) throw new InvalidOperationException("Could not find repository transaction manager.");
            transaction.Begin();

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception==null)
            {
                transaction.Commit();
            } else
            {
                transaction.Rollback();
            }
            
            base.OnActionExecuted(filterContext);
        }
    }
}

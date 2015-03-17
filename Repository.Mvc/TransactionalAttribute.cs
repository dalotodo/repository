using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
            // Credits to: http://stackoverflow.com/questions/737101/asp-net-mvc-how-to-handle-exception-in-json-action-return-json-error-info-b
            Exception exception = filterContext.Exception ?? GetExceptionFromHttpContext(filterContext.HttpContext);

            if (exception==null)
            {
                transaction.Commit();
            } else
            {
                transaction.Rollback();
            }

            TryToClearErrorFromHttpContext(filterContext.Result, filterContext.HttpContext);

            base.OnActionExecuted(filterContext);
        }

        private Exception GetExceptionFromHttpContext(HttpContextBase httpContext)
        {
            // Testing does not use HttpContext
            if (httpContext == null) return null;

            // Try to return error from HttpContext
            Exception result = null;

            try
            {
                result = httpContext.Error;
            }
            catch (NotImplementedException)
            {
                // Thrown by EmptyHttpContext
            }           
            catch
            {
                throw;
            }

            return result;
        }

        private void TryToClearErrorFromHttpContext(ActionResult result, HttpContextBase httpContext )
        {
            if (httpContext != null) return;

            try
            {
                if ((result != null) && (httpContext.Error != null))
                {
                    httpContext.ClearError();
                }
            } catch
            {
                // Safely ignore
            }
        }
    }
}

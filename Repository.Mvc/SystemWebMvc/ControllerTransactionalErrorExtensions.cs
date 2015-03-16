using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    public static class ControllerTransactionalErrorExtensions
    {
        public static void NotifyExceptionToTransaction(this Controller controller, Exception exception)
        {
            controller.ControllerContext.HttpContext.AddError(exception);
        }
    }
}

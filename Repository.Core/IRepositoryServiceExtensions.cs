using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public static class IRepositoryServiceExtensions
    {
        public static void Add<TModel>(this IRepositoryService<TModel> service, params TModel[] items)
            where TModel : class
        {
            service.Add(items as IEnumerable<TModel>);
        }

        public static void Update<TModel>(this IRepositoryService<TModel> service, params TModel[] items)
            where TModel : class
        {
            service.Update(items as IEnumerable<TModel>);
        }

        public static void Delete<TModel>(this IRepositoryService<TModel> service, params TModel[] items)
            where TModel : class
        {
            service.Delete(items as IEnumerable<TModel>);
        }
    }
}

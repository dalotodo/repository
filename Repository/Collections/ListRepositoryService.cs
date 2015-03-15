using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Collections
{
    public class ListRepositoryService<TModel> : InMemoryRepositoryService<List<TModel>, TModel>
        where TModel : class
    {
        public ListRepositoryService(): this(new List<TModel>())
        {
        }

        public ListRepositoryService(List<TModel> collection): base(collection)
        {
        }
    }
}

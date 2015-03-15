using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Decorators
{
    /// <summary>
    /// Decorates a repository service to add some sort of functionality or create a service stack
    /// </summary>
    /// <typeparam name="TModel">Type of the model stored into the repository</typeparam>
    public class RepositoryDecoratorService<TModel> : IRepositoryService<TModel>
        where TModel : class
    {
        private readonly IRepositoryService<TModel> decorated;

        public RepositoryDecoratorService( IRepositoryService<TModel> decorated)
        {
            if (decorated == null) throw new ArgumentNullException("decorated");
            this.decorated = decorated;
        }

        public virtual void Add(IEnumerable<TModel> items)
        {
            if (items == null) throw new ArgumentNullException("items");
            decorated.Add(items);
        }

        public virtual void Update(IEnumerable<TModel> items)
        {
            if (items == null) throw new ArgumentNullException("items");
            decorated.Update(items);
        }

        public virtual void Delete(IEnumerable<TModel> items)
        {
            if (items == null) throw new ArgumentNullException("items");
            decorated.Delete(items);
        }

        public virtual IQueryable<TModel> Query()
        {            
            return decorated.Query();
        }
    }
}

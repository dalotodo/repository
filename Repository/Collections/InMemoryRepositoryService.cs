using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Collections
{
    /// <summary>
    /// Implements an <see cref="IRepositoryService"/> into an in-memory collection
    /// </summary>
    /// <typeparam name="TCollection">Type of the collection</typeparam>
    /// <typeparam name="TModel">Type of the model</typeparam>
    public class InMemoryRepositoryService<TCollection,TModel> : IRepositoryService<TModel>, IDisposable
        where TModel : class
        where TCollection : ICollection<TModel>
    {
        private readonly TCollection collection;

        public InMemoryRepositoryService(TCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            this.collection = collection;
        }

        public virtual void Add(IEnumerable<TModel> items)
        {
            if (items == null) throw new ArgumentNullException("items");
            foreach( var item in items )
            {
                collection.Add(item);
            }
        }

        public virtual void Update(IEnumerable<TModel> items)
        {
            // Nothing to do. Items are already updated (as they are reference types)
        }

        public virtual void Delete(IEnumerable<TModel> items)
        {
            if (items == null) throw new ArgumentNullException("items");
            foreach (var item in items)
            {
                collection.Remove(item);
            }
        }

        public IQueryable<TModel> Query()
        {
            return collection.AsQueryable();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Do nothing
        }
    }

    public class InMemoryRepositoryService<TModel> : InMemoryRepositoryService<ICollection<TModel>, TModel>
        where TModel : class
    {
        public InMemoryRepositoryService(ICollection<TModel> collection): base(collection)
        {

        }
    }
    
}

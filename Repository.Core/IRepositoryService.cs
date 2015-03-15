using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    /// <summary>
    /// Defines a repository service. As such it incorporates Add, Update and Delete operations to the <see cref="IQueryService"/>
    /// </summary>
    /// <typeparam name="TModel">Type of the model</typeparam>
    public interface IRepositoryService<TModel> : IQueryService<TModel>
        where TModel : class
    {
        /// <summary>
        /// Adds a collection of items to the repository
        /// </summary>
        /// <param name="items">Items to add to the repository</param>
        void Add(IEnumerable<TModel> items);

        /// <summary>
        /// Updates a repository to store changes made to the given collection of items
        /// </summary>
        /// <param name="items">Items updated</param>
        void Update(IEnumerable<TModel> items);


        /// <summary>
        /// Removes a colleciton of items from the repository
        /// </summary>
        /// <param name="items">Items to remove</param>
        void Delete(IEnumerable<TModel> items);
    }
}

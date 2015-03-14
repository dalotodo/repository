using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFBoost
{
    /// <summary>
    /// Interface that must be implemented by a service that supports general or predefined queries over a model
    /// </summary>
    /// <typeparam name="TModel">Type of the queried model</typeparam>
    public interface IQueryService<TModel> where TModel : class
    {
        /// <summary>
        /// Returns a queryable collection of items of type TModel
        /// </summary>
        /// <returns>The queryable collection of items</returns>       
        IQueryable<TModel> Query();
    }
}

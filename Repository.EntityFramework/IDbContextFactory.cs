using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IDbContextFactory
    {
        /// <summary>
        /// Creates a DbContext of the given type
        /// </summary>
        /// <param name="type">Runtime type of the context. It must inherit from DbContext</param>
        /// <returns>A DbContext of the given type as stated by the implementation</returns>
        /// <exception cref="InvalidOperationException">Thrown if the given type is not inherit from DbContext type</exception>
        DbContext CreateContext(Type type);
    }

    public static class IDbContextFactoryExtensions
    {
        /// <summary>
        /// Creates a DB Context of type TContext
        /// </summary>
        /// <typeparam name="TContext">Type of the DB Context</typeparam>
        /// <returns>A DbContext of type TContext as stated by the implementation</returns>
        public static TContext CreateContext<TContext>(this IDbContextFactory factory) where TContext : DbContext
        {
            return (TContext)factory.CreateContext(typeof(TContext));
        }
    }
}

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
        /// Creates a DB Context of type TContext
        /// </summary>
        /// <typeparam name="TContext">Type of the DB Context</typeparam>
        /// <returns>A DbContext of type TContext as stated by the implementation</returns>
        TContext CreateContext<TContext>() where TContext : DbContext;
    }
}

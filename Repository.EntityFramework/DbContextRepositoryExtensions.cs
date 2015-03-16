using Repository.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class DbContextRepositoryServiceBuilder<TContext> where TContext : DbContext
    {
        private readonly TContext db;

        public DbContextRepositoryServiceBuilder(TContext db)
        {
            if (db==null) throw new ArgumentNullException("db");
            this.db = db;
        }

        /// <summary>
        /// Creates a DbContextRepositoryService backed by DB Context of type TContext 
        /// </summary>
        /// <typeparam name="TModel">Type of the model managed by the repository</typeparam>
        /// <returns>A DbContextRepositoryService for type TModel backed by DB Context of type TContext </returns>
        public DbContextRepositoryService<TContext,TModel> OfType<TModel>() where TModel : class
        {
            return new DbContextRepositoryService<TContext, TModel>(db);
        }

        /// <summary>
        ///  Creates a DbContextRepositoryService for type TModel backed by DB Context of type TContext 
        /// </summary>
        /// <typeparam name="TModel">Type of the model managed by the repository</typeparam>
        /// <param name="config">Repository options configuration callback</param>
        /// <returns>A DbContextRepositoryService for type TModel backed by DB Context of type TContext configured by confic action</returns>
        public DbContextRepositoryService<TContext, TModel> OfType<TModel>(Action<DbContextRepositoryOptions> config) where TModel : class
        {
            var options = new DbContextRepositoryOptions();
            if (config != null) config(options);
            return new DbContextRepositoryService<TContext, TModel>(db, options);
        }
    }

    public static class DbContextRepositoryExtensions
    {
        public static DbContextRepositoryServiceBuilder<TContext> CreateRepositoryService<TContext>(this TContext db) where TContext : DbContext
        {
            return new DbContextRepositoryServiceBuilder<TContext>(db);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFBoost.EntityFramework
{
    public class DbContextRepositoryOptions
    {
        /// <summary>
        /// Sets the option to tell the <see cref="DbContextRepository"/> that it should use added entities in queries
        /// </summary>
        public bool IncludeAddedItemsInQuery { get; set; }

        /// <summary>
        /// Sets the option to tell the <see cref="DbContextRepository"/> that it should use removed entities in queries
        /// </summary>
        public bool ExcludeRemovedItemsInQuery { get; set; }


        private static readonly Lazy<DbContextRepositoryOptions> _default = new Lazy<DbContextRepositoryOptions>(
            () => new DbContextRepositoryOptions
        {
            IncludeAddedItemsInQuery = true,
            ExcludeRemovedItemsInQuery = true
        });

        /// <summary>
        /// Default DbContextRepositoryOptions
        /// </summary>
        public static DbContextRepositoryOptions Default { get { return _default.Value; } }
    }
}

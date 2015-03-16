using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Tests.Utils
{
    public static class DbContextTestExtensions
    {
        public static void DropAndCreateDatabase( this DbContext db, Action<DbContext> seed  )
        {
            if (db.Database.Exists()) db.Database.Delete();
            db.Database.Create();
            if (seed != null) seed(db);
        }

        public static void DropAndCreateDatabase( this DbContext db )
        {
            DropAndCreateDatabase(db, seed: null);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    /// <summary>
    /// Manages transactions available for the IRepositoryService
    /// </summary>
    public interface IRepositoryTransactionManager : IDisposable
    {
        void Begin();
        void Commit();
        void Rollback();
    }
}

using System;

namespace Saga.Catalog.Worker.Rollback
{
    public interface IReduceQtyInCatalogRollbackWorker
    {
        void Run();
    }
}
using System;

namespace Saga.Customer.Worker.Rollback
{
    public interface IUpdateCustomerAmountRollbackWorker
    {
        void Run();
    }
}
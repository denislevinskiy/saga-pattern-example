using System;

namespace Saga.Order.Worker.Commit
{
    public interface ICreateOrderWorker
    {
        void Run();
    }
}
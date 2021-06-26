using System;
using Saga.Core.Command;
using Saga.Core.DTO;
using Saga.Customer.Repo;
using Saga.Infra.Messaging;
using Saga.Messaging.Primitive;

namespace Saga.Customer.Worker.Rollback
{
    public sealed class UpdateCustomerAmountRollbackWorker : IUpdateCustomerAmountRollbackWorker
    {
        private readonly ICustomerRepo _repo;
        private readonly IMessageBrokerFactory _messageBrokerFactory;

        public UpdateCustomerAmountRollbackWorker(ICustomerRepo repo, IMessageBrokerFactory messageBrokerFactory)
        {
            _repo = repo;
            _messageBrokerFactory = messageBrokerFactory;
        }

        public void Run()
        {
            var inputBroker = _messageBrokerFactory.GetPullBroker<CustomerAmountInfo>(MessageType.Request, CommandType.Rollback);
            var outputBroker = _messageBrokerFactory.GetPushBroker<CustomerAmountInfo>(MessageType.Success, CommandType.Rollback);
            var errorBroker = _messageBrokerFactory.GetPushBroker<Exception>(MessageType.Error, CommandType.Rollback);

            inputBroker.MessageReceived += async (sender, e) =>
            {
                try
                {
                    e.OrdersAmount = -1 * Math.Abs(e.OrdersAmount);
                    await _repo.UpdateAsync(e);
                    outputBroker.PushMessage(e);
                }
                catch (Exception ex)
                {
                    errorBroker.PushMessage(ex);
                }
            };
            
            inputBroker.Run();
        }
    }
}
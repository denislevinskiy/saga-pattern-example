using System;
using Saga.Core.Command;
using Saga.Core.DTO;
using Saga.Customer.Repo;
using Saga.Infra.Messaging;
using Saga.Messaging.Primitive;

namespace Saga.Customer.Worker.Commit
{
    public sealed class UpdateCustomerAmountWorker : IUpdateCustomerAmountWorker
    {
        private readonly ICustomerRepo _repo;
        private readonly IMessageBrokerFactory _messageBrokerFactory;

        public UpdateCustomerAmountWorker(ICustomerRepo repo, IMessageBrokerFactory messageBrokerFactory)
        {
            _repo = repo;
            _messageBrokerFactory = messageBrokerFactory;
        }

        public void Run()
        {
            var inputBroker = _messageBrokerFactory.GetPullBroker<CustomerAmountInfo>(MessageType.Request, CommandType.Commit);
            var outputBroker = _messageBrokerFactory.GetPushBroker<CustomerAmountInfo>(MessageType.Success, CommandType.Commit);
            var errorBroker = _messageBrokerFactory.GetPushBroker<Exception>(MessageType.Error, CommandType.Commit);

            inputBroker.MessageReceived += async (_, e) =>
            {
                try
                {
                    e.OrdersAmount = Math.Abs(e.OrdersAmount);
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
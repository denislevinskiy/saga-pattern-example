using System;
using Saga.Core.Command;
using Saga.Core.DTO;
using Saga.Core.DTO.Error;
using Saga.Customer.Repo;
using Saga.Infra.Abstractions;
using Saga.Infra.Abstractions.Primitive;

namespace Saga.Customer.Worker.Commit
{
    public sealed class UpdateCustomerAmountWorker : IUpdateCustomerAmountWorker
    {
        private readonly ICustomerRepo _repo;
        private readonly IMessageBrokerFactory _messageBrokerFactory;

        public UpdateCustomerAmountWorker(
            ICustomerRepo repo, 
            IMessageBrokerFactory messageBrokerFactory)
        {
            _repo = repo;
            _messageBrokerFactory = messageBrokerFactory;
        }

        public void Run()
        {
            var inputBroker = _messageBrokerFactory.GetPullBroker<CustomerAmountInfo>(MessageType.Request, CommandType.Commit);
            var outputBroker = _messageBrokerFactory.GetPushBroker<CustomerAmountInfo>(MessageType.Success, CommandType.Commit);
            var errorBroker = _messageBrokerFactory.GetPushBroker<UpdateCustomerAmountErrorInfo>(MessageType.Error, CommandType.Commit);

            inputBroker.MessageReceived += async (_, e) =>
            {
                try
                {
                    e.OrdersAmount = e.OrdersAmount;
                    await _repo.IncreaseOrdersAmountAsync(e);
                    outputBroker.PushMessage(e);
                }
                catch (Exception ex)
                {
                    errorBroker.PushMessage(new UpdateCustomerAmountErrorInfo
                    {
                        Correlation = e.Correlation,
                        Exception = ex
                    });
                }
            };
            
            inputBroker.Run();
        }
    }
}
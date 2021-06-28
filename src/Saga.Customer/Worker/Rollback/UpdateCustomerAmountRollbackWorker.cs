using System;
using Saga.Core.Command;
using Saga.Core.DTO;
using Saga.Core.DTO.Error;
using Saga.Customer.Repo;
using Saga.Infra.Abstractions;
using Saga.Infra.Abstractions.Primitive;

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
            var errorBroker = _messageBrokerFactory.GetPushBroker<UpdateCustomerAmountErrorInfo>(MessageType.Error, CommandType.Rollback);

            inputBroker.MessageReceived += async (_, e) =>
            {
                try
                {
                    e.OrdersAmount = e.OrdersAmount;
                    await _repo.DecreaseOrdersAmountAsync(e);
                    outputBroker.PushMessage(e);
                }
                catch (Exception ex)
                {
                    errorBroker.PushMessage(new UpdateCustomerAmountErrorInfo()
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
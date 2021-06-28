using System;
using Saga.Core.Command;
using Saga.Core.DTO;
using Saga.Core.DTO.Error;
using Saga.Infra.Abstractions;
using Saga.Infra.Abstractions.Primitive;
using Saga.Order.Repo;

namespace Saga.Order.Worker.Rollback
{
    public sealed class CreateOrderRollbackWorker : ICreateOrderRollbackWorker
    {
        private readonly IOrderRepo _repo;
        private readonly IMessageBrokerFactory _messageBrokerFactory;
        
        public CreateOrderRollbackWorker(IOrderRepo repo, IMessageBrokerFactory messageBrokerFactory)
        {
            _repo = repo;
            _messageBrokerFactory = messageBrokerFactory;
        }

        public void Run()
        {
            var inputBroker = _messageBrokerFactory.GetPullBroker<OrderInfo>(MessageType.Request, CommandType.Rollback);
            var outputBroker = _messageBrokerFactory.GetPushBroker<OrderInfo>(MessageType.Success, CommandType.Rollback);
            var errorBroker = _messageBrokerFactory.GetPushBroker<CreateOrderErrorInfo>(MessageType.Error, CommandType.Rollback);
            
            inputBroker.MessageReceived += async (_, e) =>
            {
                try
                {
                    await _repo.DeleteAsync(e);
                    outputBroker.PushMessage(e);
                }
                catch (Exception ex)
                {
                    errorBroker.PushMessage(new CreateOrderErrorInfo()
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
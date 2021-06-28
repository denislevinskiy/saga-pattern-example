using System;
using Saga.Core.Command;
using Saga.Core.DTO;
using Saga.Core.DTO.Error;
using Saga.Infra.Abstractions;
using Saga.Infra.Abstractions.Primitive;
using Saga.Order.Repo;

namespace Saga.Order.Worker.Commit
{
    public sealed class CreateOrderWorker : ICreateOrderWorker
    {
        private readonly IOrderRepo _repo;
        private readonly IMessageBrokerFactory _messageBrokerFactory;
        
        public CreateOrderWorker(IOrderRepo repo, IMessageBrokerFactory messageBrokerFactory)
        {
            _repo = repo;
            _messageBrokerFactory = messageBrokerFactory;
        }

        public void Run()
        {
            var inputBroker = _messageBrokerFactory.GetPullBroker<OrderInfo>(MessageType.Request, CommandType.Commit);
            var outputBroker = _messageBrokerFactory.GetPushBroker<OrderInfo>(MessageType.Success, CommandType.Commit);
            var errorBroker = _messageBrokerFactory.GetPushBroker<CreateOrderErrorInfo>(MessageType.Error, CommandType.Commit);
            
            inputBroker.MessageReceived += async (_, e) =>
            {
                try
                {
                    await _repo.AddAsync(e);
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
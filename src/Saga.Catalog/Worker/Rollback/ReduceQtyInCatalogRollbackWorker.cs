using System;
using System.Linq;
using Saga.Catalog.Repo;
using Saga.Core.Command;
using Saga.Core.DTO;
using Saga.Core.DTO.Error;
using Saga.Infra.Abstractions;
using Saga.Infra.Abstractions.Primitive;

namespace Saga.Catalog.Worker.Rollback
{
    public sealed class ReduceQtyInCatalogRollbackWorker : IReduceQtyInCatalogRollbackWorker
    {
        private readonly ICatalogRepo _repo;
        private readonly IMessageBrokerFactory _messageBrokerFactory;
        
        public ReduceQtyInCatalogRollbackWorker(
            ICatalogRepo repo, 
            IMessageBrokerFactory messageBrokerFactory)
        {
            _repo = repo;
            _messageBrokerFactory = messageBrokerFactory;
        }

        public void Run()
        {
            var inputBroker = _messageBrokerFactory.GetPullBroker<CatalogInfo>(MessageType.Request, CommandType.Rollback);
            var outputBroker = _messageBrokerFactory.GetPushBroker<CatalogInfo>(MessageType.Success, CommandType.Rollback);
            var errorBroker = _messageBrokerFactory.GetPushBroker<UpdateCatalogErrorInfo>(MessageType.Error, CommandType.Rollback);

            inputBroker.MessageReceived += async (_, e) =>
            {
                try
                {
                    e.Items = e.Items.Select(c => new CatalogInfo.CatalogItemInfo
                    {
                        Id = c.Id,
                        QtyInOrder = c.QtyInOrder,
                    }).ToList();
                    
                    await _repo.IncreaseItemsQtyAsync(e.Items);
                    outputBroker.PushMessage(e);
                }
                catch (Exception ex)
                {
                    errorBroker.PushMessage(new UpdateCatalogErrorInfo()
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
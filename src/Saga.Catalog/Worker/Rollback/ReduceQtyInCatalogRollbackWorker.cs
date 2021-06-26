using System;
using System.Collections.Generic;
using System.Linq;
using Saga.Catalog.Repo;
using Saga.Core.Command;
using Saga.Core.DTO;
using Saga.Infra.Messaging;
using Saga.Messaging.Primitive;

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
            var inputBroker = _messageBrokerFactory.GetPullBroker<List<CatalogItemInfo>>(MessageType.Request, CommandType.Rollback);
            var outputBroker = _messageBrokerFactory.GetPushBroker<List<CatalogItemInfo>>(MessageType.Success, CommandType.Rollback);
            var errorBroker = _messageBrokerFactory.GetPushBroker<Exception>(MessageType.Error, CommandType.Rollback);

            inputBroker.MessageReceived += async (_, e) =>
            {
                try
                {
                    e = e.Select(c => new CatalogItemInfo
                    {
                        Id = c.Id,
                        QtyInOrder = Math.Abs(c.QtyInOrder),
                    }).ToList();
                    
                    await _repo.UpdateItemsAsync(e);
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
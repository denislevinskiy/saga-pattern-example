using System;
using System.Collections.Generic;
using System.Linq;
using Saga.Catalog.Repo;
using Saga.Core.Command;
using Saga.Core.DTO;
using Saga.Infra.Messaging;
using Saga.Messaging.Primitive;

namespace Saga.Catalog.Worker.Commit
{
    public sealed class ReduceQtyInCatalogWorker : IReduceQtyInCatalogWorker
    {
        private readonly ICatalogRepo _repo;
        private readonly IMessageBrokerFactory _messageBrokerFactory;
        
        public ReduceQtyInCatalogWorker(
            ICatalogRepo repo, 
            IMessageBrokerFactory messageBrokerFactory)
        {
            _repo = repo;
            _messageBrokerFactory = messageBrokerFactory;
        }

        public void Run()
        {
            var inputBroker = _messageBrokerFactory.GetPullBroker<List<CatalogItemInfo>>(MessageType.Request, CommandType.Commit);
            var outputBroker = _messageBrokerFactory.GetPushBroker<List<CatalogItemInfo>>(MessageType.Success, CommandType.Commit);
            var errorBroker = _messageBrokerFactory.GetPushBroker<Exception>(MessageType.Error, CommandType.Commit);

            inputBroker.MessageReceived += async (_, e) =>
            {
                try
                {
                    e = e.Select(c => new CatalogItemInfo
                    {
                        Id = c.Id,
                        QtyInOrder = -1 * Math.Abs(c.QtyInOrder),
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
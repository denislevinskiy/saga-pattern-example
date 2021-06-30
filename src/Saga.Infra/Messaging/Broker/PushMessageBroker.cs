using System;
using System.Collections.Concurrent;
using Saga.Infra.Abstractions.Broker;

namespace Saga.Infra.Messaging.Broker
{
    public sealed class PushMessageBroker<TMessage> : IPushMessageBroker<TMessage>
    {
        private readonly ConcurrentQueue<TMessage> _queue;

        public PushMessageBroker(ConcurrentQueue<TMessage> queue)
        {
            _queue = queue;
        }
        
        public void PushMessage(TMessage e)
        {
            PushToQueue(e);
        }

        private void PushToQueue(TMessage e)
        {
            try
            {
                _queue.Enqueue(e);
            }
            catch (Exception ex)
            {
                // exception handling goes here
            }
        }
    }
}
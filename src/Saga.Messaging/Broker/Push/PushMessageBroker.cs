using System;
using System.Collections.Concurrent;

namespace Saga.Messaging.Broker.Push
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
                // TODO: add retry
            }
            catch (Exception ex)
            {
                // TODO: exception handling
            }
        }
    }
}
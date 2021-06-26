using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Saga.Messaging.Broker.Pull
{
    public sealed class PullMessageBroker<TMessage> : IPullMessageBroker<TMessage>
    {
        private readonly ConcurrentQueue<TMessage> _queue;
        
        public event EventHandler<TMessage> MessageReceived;
        
        public PullMessageBroker(ConcurrentQueue<TMessage> queue)
        {
            _queue = queue;
        }
        
        public void Run()
        {
            var listener = new Thread(_ => StartWatchingQueue()) {IsBackground = true};
            listener.Start();
        }
        
        private void StartWatchingQueue()
        {
            while (true)
            {
                Thread.Sleep(500);
                try
                {
                    if (!_queue.IsEmpty)
                    {
                        if (_queue.TryDequeue(out var message))
                        {
                            MessageReceived?.Invoke(this, message);
                        }
                        // TODO: add retry
                    }
                }
                catch (Exception ex)
                {
                    // TODO: exception handling
                }
            }
        }
    }
}
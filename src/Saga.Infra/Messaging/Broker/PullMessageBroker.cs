using System;
using System.Collections.Concurrent;
using System.Threading;
using Saga.Infra.Abstractions.Broker;

namespace Saga.Infra.Messaging.Broker
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
                try
                {
                    if (!_queue.IsEmpty)
                    {
                        if (_queue.TryDequeue(out var message))
                        {
                            MessageReceived?.Invoke(this, message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // exception handling goes here
                }
                Thread.Sleep(100);
            }
        }
    }
}
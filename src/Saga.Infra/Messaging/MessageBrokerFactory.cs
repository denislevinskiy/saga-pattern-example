using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Saga.Core.Command;
using Saga.Infra.Abstractions;
using Saga.Infra.Abstractions.Broker;
using Saga.Infra.Abstractions.Primitive;
using Saga.Infra.Messaging.Broker;

namespace Saga.Infra.Messaging
{
    public sealed class MessageBrokerFactory : IMessageBrokerFactory
    {
        private static readonly Dictionary<(Type, MessageType, CommandType), object> _queuePool = new();
        
        public IPullMessageBroker<TMessage> GetPullBroker<TMessage>(MessageType messageType, CommandType commandType)
        {
            return new PullMessageBroker<TMessage>(GetQueueFromPool<TMessage>(messageType, commandType));
        }

        public IPushMessageBroker<TMessage> GetPushBroker<TMessage>(MessageType messageType, CommandType commandType)
        {
            return new PushMessageBroker<TMessage>(GetQueueFromPool<TMessage>(messageType, commandType));
        }
        
        private static ConcurrentQueue<TMessage> GetQueueFromPool<TMessage>(MessageType messageType, CommandType commandType)
        {
            if (!_queuePool.ContainsKey((typeof(TMessage), messageType, commandType)))
            {
                _queuePool.Add((typeof(TMessage), messageType, commandType), new ConcurrentQueue<TMessage>());
            }
            return (ConcurrentQueue<TMessage>) _queuePool[(typeof(TMessage), messageType, commandType)];
        }
    }
}
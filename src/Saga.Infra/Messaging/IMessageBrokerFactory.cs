using Saga.Core.Command;
using Saga.Core.DTO;
using Saga.Messaging.Broker.Pull;
using Saga.Messaging.Broker.Push;
using Saga.Messaging.Primitive;

namespace Saga.Infra.Messaging
{
    public interface IMessageBrokerFactory
    {
        IPullMessageBroker<TMessage> GetPullBroker<TMessage>(MessageType messageType, CommandType commandType);
        
        IPushMessageBroker<TMessage> GetPushBroker<TMessage>(MessageType messageType, CommandType commandType);
    }
}
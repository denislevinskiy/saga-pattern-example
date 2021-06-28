using Saga.Core.Command;
using Saga.Infra.Abstractions.Broker;
using Saga.Infra.Abstractions.Primitive;

namespace Saga.Infra.Abstractions
{
    public interface IMessageBrokerFactory
    {
        IPullMessageBroker<TMessage> GetPullBroker<TMessage>(MessageType messageType, CommandType commandType);
        
        IPushMessageBroker<TMessage> GetPushBroker<TMessage>(MessageType messageType, CommandType commandType);
    }
}
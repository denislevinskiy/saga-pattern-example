using System;

namespace Saga.Messaging.Broker.Pull
{
    public interface IPullMessageBroker<TMessage>
    {
        event EventHandler<TMessage> MessageReceived;

        public void Run();
    }
}
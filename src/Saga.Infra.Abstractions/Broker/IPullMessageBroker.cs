using System;

namespace Saga.Infra.Abstractions.Broker
{
    public interface IPullMessageBroker<TMessage>
    {
        event EventHandler<TMessage> MessageReceived;

        public void Run();
    }
}
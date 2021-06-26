namespace Saga.Messaging.Broker.Push
{
    public interface IPushMessageBroker<in TMessage>
    {
        void PushMessage(TMessage e);
    }
}
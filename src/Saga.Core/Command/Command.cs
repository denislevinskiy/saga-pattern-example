namespace Saga.Core.Command
{
    public abstract class Command<TPayload>
    {
        public TPayload Payload { get; init; }

        public abstract CommandType CommandType { get; }
    }
}
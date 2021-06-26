using System.Diagnostics.CodeAnalysis;

namespace Saga.Core.Command
{
    [ExcludeFromCodeCoverage]
    public abstract class Command<TPayload>
    {
        public TPayload Payload { get; set; }

        public abstract CommandType CommandType { get; }
    }
}
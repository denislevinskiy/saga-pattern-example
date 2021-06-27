using System.Diagnostics.CodeAnalysis;
using Saga.Core.Command;
using Saga.Core.DTO;

namespace Saga.Orchestration.Command
{
    [ExcludeFromCodeCoverage]
    public sealed class CreateOrderRollbackCommand : Command<OrderInfo>
    {
        public override CommandType CommandType => CommandType.Rollback;
    }
}
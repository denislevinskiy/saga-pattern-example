using System.Diagnostics.CodeAnalysis;
using Saga.Core.DTO;

namespace Saga.Core.Command
{
    [ExcludeFromCodeCoverage]
    public sealed class CreateOrderCommand : Command<OrderInfo>
    {
        public override CommandType CommandType => CommandType.Commit;
    }
}
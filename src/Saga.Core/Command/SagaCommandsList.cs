using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Saga.Core.DTO;
using CommandType = Saga.Core.Command.CommandType;

namespace Saga.Core.Command
{
    [ExcludeFromCodeCoverage]
    public sealed class CreateOrderCommand : Command<OrderInfo>
    {
        public override CommandType CommandType => CommandType.Commit;
    }
    
    [ExcludeFromCodeCoverage]
    public sealed class CreateOrderRollbackCommand : Command<OrderInfo>
    {
        public override CommandType CommandType => CommandType.Commit;
    }
    
    [ExcludeFromCodeCoverage]
    public sealed class ReduceQtyInCatalogCommand : Command<List<CatalogItemInfo>>
    {
        public override CommandType CommandType => CommandType.Commit;
    }
    
    [ExcludeFromCodeCoverage]
    public sealed class ReduceQtyInCatalogRollbackCommand : Command<List<CatalogItemInfo>>
    {
        public override CommandType CommandType => CommandType.Rollback;
    }
    
    [ExcludeFromCodeCoverage]
    public sealed class UpdateCustomerAmountCommand : Command<CustomerAmountInfo>
    {
        public override CommandType CommandType => CommandType.Commit;
    }
    
    [ExcludeFromCodeCoverage]
    public sealed class UpdateCustomerRollbackCommand : Command<CustomerAmountInfo>
    {
        public override CommandType CommandType => CommandType.Rollback;
    }
}
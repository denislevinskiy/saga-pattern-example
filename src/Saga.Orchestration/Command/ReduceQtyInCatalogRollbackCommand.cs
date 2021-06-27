using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Saga.Core.Command;
using Saga.Core.DTO;

namespace Saga.Orchestration.Command
{
    [ExcludeFromCodeCoverage]
    public sealed class ReduceQtyInCatalogRollbackCommand : Command<CatalogInfo>
    {
        public override CommandType CommandType => CommandType.Rollback;
    }
}
using Saga.Core.DTO;

namespace Saga.Core.Command
{
    public sealed class ReduceQtyInCatalogCommand : Command<CatalogInfo>
    {
        public override CommandType CommandType => CommandType.Commit;
    }
}
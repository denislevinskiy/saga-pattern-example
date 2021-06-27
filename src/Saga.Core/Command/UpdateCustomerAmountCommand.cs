using Saga.Core.DTO;

namespace Saga.Core.Command
{
    public sealed class UpdateCustomerAmountCommand : Command<CustomerAmountInfo>
    {
        public override CommandType CommandType => CommandType.Commit;
    }
}
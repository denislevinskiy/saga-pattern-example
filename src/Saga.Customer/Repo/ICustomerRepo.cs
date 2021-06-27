using System.Threading.Tasks;
using Saga.Core.DTO;

namespace Saga.Customer.Repo
{
    public interface ICustomerRepo
    {
        Task IncreaseOrdersAmountAsync(CustomerAmountInfo item);
        
        Task DecreaseOrdersAmountAsync(CustomerAmountInfo item);
    }
}
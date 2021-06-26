using System.Threading.Tasks;
using Saga.Core.DTO;

namespace Saga.Customer.Repo
{
    public interface ICustomerRepo
    {
        Task UpdateAsync(CustomerAmountInfo item);
    }
}
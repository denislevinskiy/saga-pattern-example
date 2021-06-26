using System;
using System.Threading.Tasks;
using Saga.Core.DTO;

namespace Saga.Order.Repo
{
    public interface IOrderRepo
    {
        Task<Guid> AddAsync(OrderInfo orderInfo);
        
        Task DeleteAsync(OrderInfo orderInfo);
    }
}
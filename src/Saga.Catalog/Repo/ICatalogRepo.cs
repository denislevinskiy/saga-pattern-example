using System.Collections.Generic;
using System.Threading.Tasks;
using Saga.Core.DTO;

namespace Saga.Catalog.Repo
{
    public interface ICatalogRepo
    {
        Task ReduceItemsQtyAsync(List<CatalogInfo.CatalogItemInfo> items);
        
        Task IncreaseItemsQtyAsync(List<CatalogInfo.CatalogItemInfo> items);
    }
}
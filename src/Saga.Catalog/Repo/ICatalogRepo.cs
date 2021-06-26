using System.Collections.Generic;
using System.Threading.Tasks;
using Saga.Core.DTO;

namespace Saga.Catalog.Repo
{
    public interface ICatalogRepo
    {
        Task UpdateItemsAsync(IEnumerable<CatalogItemInfo> items);
    }
}
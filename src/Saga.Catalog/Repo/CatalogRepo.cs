using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Saga.Core.DTO;
using Saga.Infra.SQLite;

namespace Saga.Catalog.Repo
{
    public sealed class CatalogRepo : ICatalogRepo
    {
        private const string TableName = "Catalog";
        
        private readonly ISQLiteConnectionFactory _connectionFactory;

        public CatalogRepo(ISQLiteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task UpdateItemsAsync(IEnumerable<CatalogItemInfo> items)
        {
            await using var conn = _connectionFactory.OpenLocalDbConnection();
            
            await Task.WhenAll(items.Select(item => conn.ExecuteAsync(
                $@"
                    UPDATE {TableName}
                    SET
                        TimeStamp = @TimeStamp,
                        Stock = Stock + @StockChange
                    WHERE Id = @Id",
                new
                {
                    item.Id,
                    TimeStamp = DateTimeOffset.UtcNow,
                    StockChange = item.QtyInOrder,
                })));
        }
    }
}

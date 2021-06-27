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
        private const string TableName = "[Catalog]";
        
        private readonly ISQLiteConnectionFactory _connectionFactory;

        public CatalogRepo(ISQLiteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task ReduceItemsQtyAsync(List<CatalogInfo.CatalogItemInfo> items)
        {
            items.ForEach(item =>
            {
                if (item.QtyInOrder <= 0)
                {
                    throw new Exception($"Invalid order quantity: {item.QtyInOrder}");
                }
            });
            
            await using var conn = _connectionFactory.OpenLocalDbConnection();

            await Task.WhenAll(items.Select(async item =>
            {
                var stock = await conn.QuerySingleOrDefaultAsync<int>(
                    $@"
                        SELECT 
                            [Stock]
                        FROM {TableName} 
                        WHERE 
                            [Id] = @Id",
                    new {item.Id});

                if (item.QtyInOrder > stock)
                {
                    throw new Exception($"Stock is exceeded: {item.QtyInOrder} required, {stock} left");
                }
                
                return conn.ExecuteAsync(
                    $@"
                    UPDATE {TableName}
                    SET
                        TimeStamp = @TimeStamp,
                        Stock = Stock - @StockChange
                    WHERE Id = @Id",
                    new
                    {
                        item.Id,
                        TimeStamp = DateTimeOffset.UtcNow,
                        StockChange = item.QtyInOrder,
                    });
            }));
        }

        public async Task IncreaseItemsQtyAsync(List<CatalogInfo.CatalogItemInfo> items)
        {
            await using var conn = _connectionFactory.OpenLocalDbConnection();

            await Task.WhenAll(items.Select(
                async item => conn.ExecuteAsync(
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

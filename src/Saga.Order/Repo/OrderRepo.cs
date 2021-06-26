using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Saga.Core.DTO;
using Saga.Infra.SQLite;

namespace Saga.Order.Repo
{
    public sealed class OrderRepo : IOrderRepo
    {
        private const string TableName = "[Order]";
        private const string ChildTableName = "OrderDetails";

        private readonly ISQLiteConnectionFactory _connectionFactory;

        public OrderRepo(ISQLiteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Guid> AddAsync(OrderInfo orderInfo)
        {
            await using var conn = _connectionFactory.OpenLocalDbConnection();

            var tran = await conn.BeginTransactionAsync();

            var orderId = orderInfo.Id;
            var timeStamp = DateTimeOffset.UtcNow;

            try
            {
                await conn.ExecuteAsync(
                    $@"
                    INSERT INTO {TableName} 
                    (
                        Id,
                        TimeStamp,
                        CustomerId
                    )
                    VALUES
                    (
                        @Id,
                        @TimeStamp,
                        @CustomerId
                    )",
                    new
                    {
                        Id = orderId,
                        TimeStamp = timeStamp,
                        orderInfo.CustomerId,
                    },
                    tran);

                if (orderInfo.Details.Any())
                {
                    foreach (var item in orderInfo.Details)
                    {
                        await conn.ExecuteAsync(
                            $@"INSERT INTO {ChildTableName}
                            (
                                Id,
                                TimeStamp,
                                OrderId,
                                CatalogItemId,
                                Qty,
                                Amount
                            ) 
                            VALUES 
                            (
                                @Id,
                                @TimeStamp,
                                @OrderId,
                                @CatalogItemId,
                                @Qty,
                                @Amount
                            );",
                            new
                            {
                                Id = Guid.NewGuid(),
                                TimeStamp = timeStamp,
                                OrderId = orderId,
                                item.CatalogItemId,
                                item.Qty,
                                item.Amount
                            },
                            tran
                        );
                    }
                }

                await tran.CommitAsync();

                return orderId;
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(OrderInfo orderInfo)
        {
            await using var conn = _connectionFactory.OpenLocalDbConnection();
            var tran = await conn.BeginTransactionAsync();
            try
            {
                await conn.ExecuteAsync(
                    $@"DELETE FROM {ChildTableName} WHERE Id = @Id;",
                    new
                    {
                        Id = orderInfo.Id,
                    },
                    tran);

                if (orderInfo.Details.Any())
                {
                    await Task.WhenAll(orderInfo.Details.Select(item => conn.ExecuteAsync(
                        $@"DELETE FROM {ChildTableName} WHERE OrderId = @Id;",
                        new
                        {
                            OrderId = orderInfo.Id
                        },
                        tran
                    )));
                }

                await tran.CommitAsync();
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
            }
        }
    }
}
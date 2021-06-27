using System;
using System.Threading.Tasks;
using Dapper;
using Saga.Core.DTO;
using Saga.Infra.SQLite;

namespace Saga.Customer.Repo
{
    public sealed class CustomerRepo : ICustomerRepo
    {
        private const string TableName = "Customer";
        
        private readonly ISQLiteConnectionFactory _connectionFactory;

        public CustomerRepo(ISQLiteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        
        public async Task IncreaseOrdersAmountAsync(CustomerAmountInfo item)
        {
            if (item.OrdersAmount <= 0)
            {
                throw new Exception($"Invalid orders amount: {item.OrdersAmount}");
            }
            
            await using var conn = _connectionFactory.OpenLocalDbConnection();
            
            await conn.ExecuteAsync(
                $@"
                    UPDATE {TableName}
                    SET
                        TimeStamp = @TimeStamp,
                        OrdersAmount = OrdersAmount + @OrdersAmount
                    WHERE Id = @Id",
                new
                {
                    item.Id,
                    TimeStamp = DateTimeOffset.UtcNow,
                    item.OrdersAmount,
                });
        }

        public async Task DecreaseOrdersAmountAsync(CustomerAmountInfo item)
        {
            await using var conn = _connectionFactory.OpenLocalDbConnection();
            
            await conn.ExecuteAsync(
                $@"
                    UPDATE {TableName}
                    SET
                        TimeStamp = @TimeStamp,
                        OrdersAmount = OrdersAmount - @OrdersAmount
                    WHERE Id = @Id",
                new
                {
                    item.Id,
                    TimeStamp = DateTimeOffset.UtcNow,
                    item.OrdersAmount,
                });
        }
    }
}
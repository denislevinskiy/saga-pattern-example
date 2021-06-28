using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Saga.Infra.Abstractions;
using Saga.Orchestration.DTO;

namespace Saga.Orchestration.Repo
{
    public sealed class SagaStateRepo : ISagaStateRepo
    {
        private const string TableName = "[SagaState]";

        private readonly IDataStorageConnectionFactory _connectionFactory;

        public SagaStateRepo(IDataStorageConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task UpdateStateAsync(SagaStateInfo info)
        {
            using var conn = _connectionFactory.OpenLocalDbConnection();

            await conn.ExecuteAsync(
                $@"
                INSERT INTO {TableName} 
                (
                    Correlation,
                    TimeStamp,
                    State,
                    Info
                )
                VALUES
                (
                    @Correlation,
                    @TimeStamp,
                    @State,
                    @Info
                )",
                info);
        }

        public async Task<List<SagaStateInfo>> GetListAsync(Guid correlation)
        {
            using var conn = _connectionFactory.OpenLocalDbConnection();

            return (await conn.QueryAsync<SagaStateInfo>(
                $@"
                SELECT
                    Correlation,
                    CAST(TimeStamp AS DATETIME),
                    State,
                    Info
                FROM {TableName}
                WHERE Correlation = @Correlation",
                new {Correlation = correlation})).ToList();
        }
    }
}
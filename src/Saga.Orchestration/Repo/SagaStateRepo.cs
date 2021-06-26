using System.Threading.Tasks;
using Dapper;
using Saga.Infra.SQLite;
using Saga.Orchestration.DTO;

namespace Saga.Orchestration.Repo
{
    public sealed class SagaStateRepo : ISagaStateRepo
    {
        private const string TableName = "[SagaState]";

        private readonly ISQLiteConnectionFactory _connectionFactory;

        public SagaStateRepo(ISQLiteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task UpdateStateAsync(SagaStateInfo info)
        {
            await using var conn = _connectionFactory.OpenLocalDbConnection();

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
    }
}
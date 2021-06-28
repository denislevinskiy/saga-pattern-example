using System.Data;
using System.Data.SQLite;
using Saga.Infra.Abstractions;

namespace Saga.Infra.SQLite
{
    public sealed class SQLiteConnectionFactory : IDataStorageConnectionFactory
    {
        private const string DbFile = "saga-pattern.sqlite";
        
        public IDbConnection OpenLocalDbConnection() => new SQLiteConnection($"Data Source={DbFile}").OpenAndReturn();
    }
}
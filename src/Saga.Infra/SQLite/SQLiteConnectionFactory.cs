using System.Data.SQLite;

namespace Saga.Infra.SQLite
{
    public sealed class SQLiteConnectionFactory : ISQLiteConnectionFactory
    {
        private const string DbFile = "saga-pattern.sqlite";
        
        public SQLiteConnection OpenLocalDbConnection() => new SQLiteConnection($"Data Source={DbFile}").OpenAndReturn();
    }
}
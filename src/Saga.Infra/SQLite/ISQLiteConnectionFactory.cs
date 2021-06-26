using System.Data.SQLite;

namespace Saga.Infra.SQLite
{
    public interface ISQLiteConnectionFactory
    {
        SQLiteConnection OpenLocalDbConnection();
    }
}
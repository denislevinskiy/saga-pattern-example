using System.Data;

namespace Saga.Infra.Abstractions
{
    public interface IDataStorageConnectionFactory
    {
        IDbConnection OpenLocalDbConnection();
    }
}
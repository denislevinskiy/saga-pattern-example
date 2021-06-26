using System;
using System.Data;
using Dapper;

namespace Saga.Infra.SQLite
{
    public sealed class GuidTypeDapperHandler : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.Value = value.ToString();
        }

        public override Guid Parse(object value)
        {
            return Guid.Parse(value.ToString() ?? throw new InvalidOperationException());
        }
    }
}
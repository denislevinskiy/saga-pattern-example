using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Saga.Infra.SQLite;
using Saga.Orchestration.DTO;
using Saga.Orchestration.Repo;

namespace Saga.Tests.Integration
{
    public static class DbHelper
    {
        public static async Task<List<SagaStateInfo>> GetState(Guid correlation)
        {
            SqlMapper.AddTypeHandler(new GuidTypeDapperHandler());
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));
            var sagaStateRepo = new SagaStateRepo(new SQLiteConnectionFactory());
            return await sagaStateRepo.GetListAsync(correlation);
        }
    }
}
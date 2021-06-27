using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saga.Orchestration.DTO;

namespace Saga.Orchestration.Repo
{
    public interface ISagaStateRepo
    {
        Task UpdateStateAsync(SagaStateInfo item);

        Task<List<SagaStateInfo>> GetListAsync(Guid correlation);
    }
}
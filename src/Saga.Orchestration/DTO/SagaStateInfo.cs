using System;

namespace Saga.Orchestration.DTO
{
    public sealed class SagaStateInfo
    {
        public Guid Correlation { get; set; }
        
        public DateTimeOffset TimeStamp { get; set; }
        
        public string State { get; set; }
        
        public string Info { get; set; }
    }
}
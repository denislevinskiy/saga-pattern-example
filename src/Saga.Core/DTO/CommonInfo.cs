using System;

namespace Saga.Core.DTO
{
    public abstract class CommonInfo
    {
        public Guid Correlation { get; init; }
    }
}
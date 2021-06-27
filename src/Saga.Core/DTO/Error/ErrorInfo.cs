using System;

namespace Saga.Core.DTO.Error
{
    public abstract class ErrorInfo : CommonInfo
    {
        public Exception Exception { get; init; }
    }
}
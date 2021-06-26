using System;
using System.Diagnostics.CodeAnalysis;
using Saga.Core.Extensions;

namespace Saga.Core.DTO
{
    [ExcludeFromCodeCoverage]
    public sealed class CustomerAmountInfo
    {
        public Guid Id { get; set; }
        
        public decimal OrdersAmount { get; set; }
        
        public string ToJsonString()
        {
            return JsonExtensions.SerializeObject(this);
        }
    }
}
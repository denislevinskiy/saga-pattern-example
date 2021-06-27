using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Saga.Core.Extensions;

namespace Saga.Core.DTO
{
    [ExcludeFromCodeCoverage]
    public sealed class OrderInfo : CommonInfo
    {
        public Guid Id { get; init; }

        public Guid CustomerId { get; init; }

        public List<OrderDetailsInfo> Details { get; init; } = new();
        
        public sealed class OrderDetailsInfo
        {
            public decimal Amount { get; init; }

            public Guid CatalogItemId { get; init; }

            public int Qty { get; init; }
        }
        
        public string ToJsonString()
        {
            return JsonExtensions.SerializeObject(this);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Saga.Core.Extensions;

namespace Saga.Core.DTO
{
    [ExcludeFromCodeCoverage]
    public sealed class OrderInfo
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public List<OrderDetailsInfo> Details { get; set; } = new();
        
        public sealed class OrderDetailsInfo
        {
            public decimal Amount { get; set; }

            public Guid Id { get; set; }

            public Guid CatalogItemId { get; set; }

            public int Qty { get; set; }
        }
        
        public string ToJsonString()
        {
            return JsonExtensions.SerializeObject(this);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Saga.Core.Extensions;

namespace Saga.Core.DTO
{
    [ExcludeFromCodeCoverage]
    public sealed class CatalogInfo : CommonInfo
    {
        public List<CatalogItemInfo> Items { get; set; }
        
        public sealed class CatalogItemInfo
        {
            public Guid Id { get; init; }

            public int QtyInOrder { get; init; }
        }

        public string ToJsonString()
        {
            return JsonExtensions.SerializeObject(this);
        }
    }
}
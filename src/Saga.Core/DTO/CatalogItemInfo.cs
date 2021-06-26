using System;
using System.Diagnostics.CodeAnalysis;
using Saga.Core.Extensions;

namespace Saga.Core.DTO
{
    [ExcludeFromCodeCoverage]
    public sealed class CatalogItemInfo
    {
        public Guid Id { get; set; }

        public int QtyInOrder { get; set; }

        public string ToJsonString()
        {
            return JsonExtensions.SerializeObject(this);
        }
    }
}
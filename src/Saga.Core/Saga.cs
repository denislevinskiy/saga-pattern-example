using System;
using System.Diagnostics.CodeAnalysis;
using Saga.Core.Command;
using Saga.Core.DTO;

namespace Saga.Core
{
    [ExcludeFromCodeCoverage]
    public sealed class Saga
    {
        public Guid Correlation { get; set; }
        
        public CreateOrderCommand CreateOrderCommand { get; init; }
    
        public ReduceQtyInCatalogCommand ReduceQtyInCatalogCommand { get; init; }
    
        public UpdateCustomerAmountCommand UpdateCustomerAmountCommand { get; init; }
    }
}
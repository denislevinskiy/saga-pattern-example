using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saga.Catalog.Worker.Commit;
using Saga.Core.Command;
using Saga.Core.DTO;
using Saga.Customer.Worker.Commit;
using Saga.Orchestration;
using Saga.Order.Worker.Commit;

namespace Facade.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderSagaController : ControllerBase
    {
        private readonly ILogger<OrderSagaController> _logger;
        private readonly IOrchestrator _orchestrator;
        private readonly ICreateOrderWorker _createOrderWorker;
        private readonly IReduceQtyInCatalogWorker _reduceQtyInCatalogWorker;
        private readonly IUpdateCustomerAmountWorker _updateCustomerAmountWorker;

        public OrderSagaController(
            ILogger<OrderSagaController> logger, 
            IOrchestrator orchestrator, 
            ICreateOrderWorker createOrderWorker, 
            IReduceQtyInCatalogWorker reduceQtyInCatalogWorker, 
            IUpdateCustomerAmountWorker updateCustomerAmountWorker)
        {
            _logger = logger;
            _orchestrator = orchestrator;
            _createOrderWorker = createOrderWorker;
            _reduceQtyInCatalogWorker = reduceQtyInCatalogWorker;
            _updateCustomerAmountWorker = updateCustomerAmountWorker;
        }

        [HttpPost("run")]
        public async Task<IActionResult> Run()
        {
            var correlation = Guid.NewGuid();
            
            _createOrderWorker.Run();
            _reduceQtyInCatalogWorker.Run();
            _updateCustomerAmountWorker.Run();
            
            await _orchestrator.Run(new Saga.Core.Saga
            {
                CreateOrderCommand = new CreateOrderCommand
                {
                    Payload = new OrderInfo
                    {
                        Id = correlation,
                        Details = new List<OrderInfo.OrderDetailsInfo>
                        {
                            new()
                            {
                                Amount = 50,
                                Id = Guid.NewGuid(),
                                Qty = 1,
                                CatalogItemId = Guid.Parse("7b31d64c-6aab-4632-85bd-7bd38f09aa76"),
                            },
                        },
                        CustomerId = Guid.Parse("2d5fe5ae-9f9e-414a-8425-529d075ac86b"),
                    },
                },
                ReduceQtyInCatalogCommand = new ReduceQtyInCatalogCommand
                {
                    Payload = new List<CatalogItemInfo>
                    {
                        new()
                        {
                            Id = Guid.Parse("7b31d64c-6aab-4632-85bd-7bd38f09aa76"),
                            QtyInOrder = 1,
                        }
                    }
                },
                UpdateCustomerAmountCommand = new UpdateCustomerAmountCommand
                {
                    Payload = new CustomerAmountInfo
                    {
                        Id = Guid.Parse("2d5fe5ae-9f9e-414a-8425-529d075ac86b"),
                        OrdersAmount = 50,
                    }
                },
            });
            
            return await Task.FromResult(Accepted());
        }
    }
}
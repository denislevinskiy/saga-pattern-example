using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Saga.Catalog.Worker.Commit;
using Saga.Catalog.Worker.Rollback;
using Saga.Customer.Worker.Commit;
using Saga.Customer.Worker.Rollback;
using Saga.Orchestration;
using Saga.Order.Worker.Commit;
using Saga.Order.Worker.Rollback;

namespace Facade.Api.Controllers
{
    [ApiController]
    [Route("order-saga")]
    public class OrderSagaController : ControllerBase
    {
        private readonly IOrchestrator _orchestrator;
        private readonly ICreateOrderWorker _createOrderWorker;
        private readonly ICreateOrderRollbackWorker _createOrderRollbackWorker;
        private readonly IReduceQtyInCatalogWorker _reduceQtyInCatalogWorker;
        private readonly IReduceQtyInCatalogRollbackWorker _reduceQtyInCatalogRollbackWorker;
        private readonly IUpdateCustomerAmountWorker _updateCustomerAmountWorker;
        private readonly IUpdateCustomerAmountRollbackWorker _updateCustomerAmountRollbackWorker;

        public OrderSagaController(
            IOrchestrator orchestrator, 
            ICreateOrderWorker createOrderWorker, 
            IReduceQtyInCatalogWorker reduceQtyInCatalogWorker, 
            IUpdateCustomerAmountWorker updateCustomerAmountWorker, 
            ICreateOrderRollbackWorker createOrderRollbackWorker, 
            IReduceQtyInCatalogRollbackWorker reduceQtyInCatalogRollbackWorker, 
            IUpdateCustomerAmountRollbackWorker updateCustomerAmountRollbackWorker)
        {
            _orchestrator = orchestrator;
            _createOrderWorker = createOrderWorker;
            _reduceQtyInCatalogWorker = reduceQtyInCatalogWorker;
            _updateCustomerAmountWorker = updateCustomerAmountWorker;
            _createOrderRollbackWorker = createOrderRollbackWorker;
            _reduceQtyInCatalogRollbackWorker = reduceQtyInCatalogRollbackWorker;
            _updateCustomerAmountRollbackWorker = updateCustomerAmountRollbackWorker;
        }

        [HttpPost("run")]
        public async Task<IActionResult> Run(Saga.Core.Saga saga)
        {
            _createOrderWorker.Run();
            _reduceQtyInCatalogWorker.Run();
            _updateCustomerAmountWorker.Run();
            _createOrderRollbackWorker.Run();
            _reduceQtyInCatalogRollbackWorker.Run();
            _updateCustomerAmountRollbackWorker.Run();
            
            await _orchestrator.Run(saga);
            
            return await Task.FromResult(Accepted());
        }
    }
}
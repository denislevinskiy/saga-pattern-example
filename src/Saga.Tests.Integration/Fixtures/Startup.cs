using Facade.Api.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Saga.Catalog.Repo;
using Saga.Catalog.Worker.Commit;
using Saga.Catalog.Worker.Rollback;
using Saga.Customer.Repo;
using Saga.Customer.Worker.Commit;
using Saga.Customer.Worker.Rollback;
using Saga.Infra.Abstractions;
using Saga.Infra.Messaging;
using Saga.Infra.SQLite;
using Saga.Orchestration;
using Saga.Orchestration.Repo;
using Saga.Order.Repo;
using Saga.Order.Worker.Commit;
using Saga.Order.Worker.Rollback;

namespace Saga.Tests.Integration.Fixtures
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      DbSetupHelper.Setup(); // create necessary DB infrastructure
            
      services.AddSingleton<IDataStorageConnectionFactory, SQLiteConnectionFactory>();
      services.AddSingleton<IMessageBrokerFactory, MessageBrokerFactory>();
            
      services.AddSingleton<IOrderRepo, OrderRepo>();
      services.AddSingleton<ICatalogRepo, CatalogRepo>();
      services.AddSingleton<ICustomerRepo, CustomerRepo>();
      services.AddSingleton<ISagaStateRepo, SagaStateRepo>();

      services.AddSingleton<IOrchestrator, Orchestrator>();
      services.AddSingleton<ICreateOrderWorker, CreateOrderWorker>();
      services.AddSingleton<ICreateOrderRollbackWorker, CreateOrderRollbackWorker>();
      services.AddSingleton<IReduceQtyInCatalogWorker, ReduceQtyInCatalogWorker>();
      services.AddSingleton<IReduceQtyInCatalogRollbackWorker, ReduceQtyInCatalogRollbackWorker>();
      services.AddSingleton<IUpdateCustomerAmountWorker, UpdateCustomerAmountWorker>();
      services.AddSingleton<IUpdateCustomerAmountRollbackWorker, UpdateCustomerAmountRollbackWorker>();
            
      services.AddControllers();
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseHttpsRedirection();
      app.UseRouting();
      app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
  }
}
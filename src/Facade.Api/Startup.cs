using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
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

namespace Facade.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Facade.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Facade.Api v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
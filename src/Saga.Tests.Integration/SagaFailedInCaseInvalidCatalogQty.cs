using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Saga.Core.Command;
using Saga.Core.DTO;
using Saga.Core.Extensions;
using Saga.Infra.SQLite;
using Saga.Orchestration.Repo;
using Saga.Tests.Integration.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace Saga.Tests.Integration
{
    [Collection("WebServerFixture")]
    public class SagaFailedInCaseInvalidCatalogQtyTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _client;

        public SagaFailedInCaseInvalidCatalogQtyTest(WebServerFixture webServerFixture, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _client = webServerFixture.HttpClient;
        }

        [Fact(DisplayName = "Saga failed in case invalid catalog qty")]
        public async Task SagaSucceed()
        {
            const string url = "/order-saga/run";
            
            var correlation = Guid.NewGuid();
            var saga = new Saga.Core.Saga
            {
                Correlation = correlation,
                CreateOrderCommand = new CreateOrderCommand
                {
                    Payload = new OrderInfo
                    {
                        Correlation = correlation,
                        Id = correlation,
                        Details = new List<OrderInfo.OrderDetailsInfo>
                        {
                            new()
                            {
                                Amount = 50,
                                Qty = 1,
                                CatalogItemId = Guid.Parse("7b31d64c-6aab-4632-85bd-7bd38f09aa76"),
                            },
                        },
                        CustomerId = Guid.Parse("2d5fe5ae-9f9e-414a-8425-529d075ac86b"),
                    },
                },
                ReduceQtyInCatalogCommand = new ReduceQtyInCatalogCommand
                {
                    Payload = new CatalogInfo()
                    {
                        Correlation = correlation,
                        Items = new List<CatalogInfo.CatalogItemInfo>()
                        {
                            new()
                            {
                                Id = Guid.Parse("7b31d64c-6aab-4632-85bd-7bd38f09aa76"),
                                QtyInOrder = 100,
                            }
                        }
                    }
                },
                UpdateCustomerAmountCommand = new UpdateCustomerAmountCommand
                {
                    Payload = new CustomerAmountInfo
                    {
                        Correlation = correlation,
                        Id = Guid.Parse("2d5fe5ae-9f9e-414a-8425-529d075ac86b"),
                        OrdersAmount = 50,
                    }
                },
            };

            var response = await _client.PostAsync(
                url, 
                new StringContent(JsonExtensions.SerializeObject(saga), 
                    Encoding.UTF8, 
                    MediaTypeNames.Application.Json));
            response.EnsureSuccessStatusCode();
            
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);

            await Task.Delay(1500); // give some time to finish all background operations

            var state = await DbHelper.GetState(correlation);
            
            state.ForEach(item => _testOutputHelper.WriteLine(
                $"{item.Correlation};{item.TimeStamp};{item.State};{item.Info}"));
            
            Assert.Equal(6, state.Count);
            
            Assert.Contains(state, item => item.State == "Begin");
            Assert.Contains(state, item => item.State == "OrderCreated");
            Assert.Contains(state, item => item.State == "CatalogUpdateFailed");
            Assert.Contains(state, item => item.State == "OrderCreationRollbackSucceed");
            Assert.Contains(state, item => item.State == "SagaFailed");
            Assert.Contains(state, item => item.State == "End");
        }
    }
}
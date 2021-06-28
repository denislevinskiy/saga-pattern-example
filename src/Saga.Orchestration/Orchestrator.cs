using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saga.Core.Command;
using Saga.Core.DTO;
using Saga.Core.DTO.Error;
using Saga.Infra.Abstractions;
using Saga.Infra.Abstractions.Broker;
using Saga.Infra.Abstractions.Primitive;
using Saga.Orchestration.Command;
using Saga.Orchestration.DTO;
using Saga.Orchestration.Repo;

namespace Saga.Orchestration
{
    public sealed class Orchestrator : IOrchestrator
    {
        private Core.Saga _saga;
        private string _state;

        private readonly IMessageBrokerFactory _messageBrokerFactory;
        private readonly ISagaStateRepo _sagaStateRepo;
        private readonly Dictionary<(Type, CommandType), object> pushBrokers = new();

        public Orchestrator(IMessageBrokerFactory messageBrokerFactory, ISagaStateRepo sagaStateRepo)
        {
            _messageBrokerFactory = messageBrokerFactory;
            _sagaStateRepo = sagaStateRepo;

            pushBrokers.Add(
                (typeof(OrderInfo), CommandType.Commit),
                messageBrokerFactory.GetPushBroker<OrderInfo>(MessageType.Request, CommandType.Commit));
            pushBrokers.Add(
                (typeof(OrderInfo), CommandType.Rollback),
                messageBrokerFactory.GetPushBroker<OrderInfo>(MessageType.Request, CommandType.Rollback));

            pushBrokers.Add(
                (typeof(CatalogInfo), CommandType.Commit),
                messageBrokerFactory.GetPushBroker<CatalogInfo>(MessageType.Request, CommandType.Commit));
            pushBrokers.Add(
                (typeof(CatalogInfo), CommandType.Rollback),
                messageBrokerFactory.GetPushBroker<CatalogInfo>(MessageType.Request, CommandType.Rollback));

            pushBrokers.Add(
                (typeof(CustomerAmountInfo), CommandType.Commit),
                messageBrokerFactory.GetPushBroker<CustomerAmountInfo>(MessageType.Request, CommandType.Commit));
            pushBrokers.Add(
                (typeof(CustomerAmountInfo), CommandType.Rollback),
                messageBrokerFactory.GetPushBroker<CustomerAmountInfo>(MessageType.Request, CommandType.Rollback));
        }

        public async Task Run(Core.Saga saga)
        {
            RunCreateOrderBrokers();
            RunReduceQtyInCatalogBrokers();
            RunUpdateCustomerAmountBrokers();

            _saga = saga;
            _saga.Correlation = _saga.Correlation == default ? Guid.NewGuid() : _saga.Correlation;
            _state = SagaState.Begin;
            
            await RunNext();
        }

        private async Task RunNext()
        {
            void ExecuteCommand<TPayload>(Command<TPayload> cmd)
            {
                var broker = (IPushMessageBroker<TPayload>) pushBrokers[(typeof(TPayload), cmd.CommandType)];
                broker.PushMessage(cmd.Payload);
            }
            
            switch (_state)
            {
                case SagaState.Begin:
                    await UpdateStateAsync();
                    ExecuteCommand(_saga.CreateOrderCommand);
                    break;
                case SagaState.OrderCreated:
                    ExecuteCommand(_saga.ReduceQtyInCatalogCommand);
                    break;
                case SagaState.OrderCreationRollbackFailed:
                case SagaState.CatalogUpdateRollbackFailed:
                case SagaState.CustomerAmountUpdateRollbackFailed:
                    _state = SagaState.SagaUnexpectedError; 
                    // an additional "on-fail"-logic can be added here
                    await RunNext();
                    break;
                case SagaState.OrderCreationFailed:
                case SagaState.OrderCreationRollbackSucceed:
                    _state = SagaState.SagaFailed;  
                    // an additional "on-cancel"-logic can be added here
                    await RunNext();
                    break;
                case SagaState.CatalogUpdated:
                    ExecuteCommand(_saga.UpdateCustomerAmountCommand);
                    break;
                case SagaState.CatalogUpdateFailed:
                case SagaState.CatalogUpdateRollbackSucceed:
                    ExecuteCommand(new CreateOrderRollbackCommand
                    {
                        Payload = _saga.CreateOrderCommand.Payload
                    });
                    break;
                case SagaState.CustomerAmountUpdated:
                    _state = SagaState.SagaSucceed;  
                    // an additional "on-success"-logic can be added here
                    await RunNext();
                    break;
                case SagaState.CustomerAmountUpdateFailed:
                case SagaState.CustomerAmountUpdateRollbackSucceed:
                    ExecuteCommand(new ReduceQtyInCatalogRollbackCommand
                    {
                        Payload = _saga.ReduceQtyInCatalogCommand.Payload
                    });
                    break;
                case SagaState.SagaSucceed:
                case SagaState.SagaFailed:
                case SagaState.SagaUnexpectedError:
                    await UpdateStateAsync();
                    _state = SagaState.End;
                    await RunNext();
                    break;
                case SagaState.End:
                    await UpdateStateAsync();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task UpdateStateAsync(string info = null)
        {
            try
            {
                await _sagaStateRepo.UpdateStateAsync(new SagaStateInfo
                {
                    Correlation = _saga.Correlation,
                    TimeStamp = DateTimeOffset.UtcNow,
                    State = _state,
                    Info = info
                });
            }
            catch (Exception ex)
            {
                // an additional exception handling can be added here; we just skip it for simplicity
            } 
        }
        
        private void RunCreateOrderBrokers()
        {
            var successBroker =
                _messageBrokerFactory.GetPullBroker<OrderInfo>(MessageType.Success, CommandType.Commit);
            successBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.OrderCreated;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            successBroker.Run();

            var errorBroker = _messageBrokerFactory.GetPullBroker<CreateOrderErrorInfo>(MessageType.Error, CommandType.Commit);
            errorBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.OrderCreationFailed;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            errorBroker.Run();
                
            var successRollbackBroker =
                _messageBrokerFactory.GetPullBroker<OrderInfo>(MessageType.Success, CommandType.Rollback);
            successRollbackBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.OrderCreationRollbackSucceed;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            successRollbackBroker.Run();

            var errorRollbackBroker = _messageBrokerFactory.GetPullBroker<CreateOrderErrorInfo>(MessageType.Error, CommandType.Rollback);
            errorRollbackBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.OrderCreationRollbackFailed;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            errorRollbackBroker.Run();
        }
        
        private void RunReduceQtyInCatalogBrokers()
        {
            var successBroker =
                _messageBrokerFactory.GetPullBroker<CatalogInfo>(MessageType.Success, CommandType.Commit);
            successBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.CatalogUpdated;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            successBroker.Run();

            var errorBroker =
                _messageBrokerFactory.GetPullBroker<UpdateCatalogErrorInfo>(MessageType.Error, CommandType.Commit);
            errorBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.CatalogUpdateFailed;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            errorBroker.Run();
                
            var successRollbackBroker =
                _messageBrokerFactory.GetPullBroker<CatalogInfo>(MessageType.Success, CommandType.Rollback);
            successRollbackBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.CatalogUpdateRollbackSucceed;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            successRollbackBroker.Run();

            var errorRollbackBroker =
                _messageBrokerFactory.GetPullBroker<UpdateCatalogErrorInfo>(MessageType.Error, CommandType.Rollback);
            errorRollbackBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.CatalogUpdateRollbackFailed;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            errorRollbackBroker.Run();
        }
        
        private void RunUpdateCustomerAmountBrokers()
        {
            var successBroker = _messageBrokerFactory.GetPullBroker<CustomerAmountInfo>(MessageType.Success, CommandType.Commit);
            successBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.CustomerAmountUpdated;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            successBroker.Run();

            var errorBroker = _messageBrokerFactory.GetPullBroker<UpdateCustomerAmountErrorInfo>(MessageType.Error, CommandType.Commit);
            errorBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.CustomerAmountUpdateFailed;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            errorBroker.Run();
                
            var successRollbackBroker = _messageBrokerFactory.GetPullBroker<CustomerAmountInfo>(MessageType.Success, CommandType.Rollback);
            successRollbackBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.CustomerAmountUpdateRollbackSucceed;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            successRollbackBroker.Run();

            var errorRollbackBroker = _messageBrokerFactory.GetPullBroker<UpdateCustomerAmountErrorInfo>(MessageType.Error, CommandType.Rollback);
            errorRollbackBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.CustomerAmountUpdateRollbackFailed;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            errorRollbackBroker.Run();
        }
    }
}
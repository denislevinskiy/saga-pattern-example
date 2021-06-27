namespace Saga.Orchestration.DTO
{
    public static class SagaState
    {
        public const string Begin = "Begin";
        public const string OrderCreated = "OrderCreated";
        public const string OrderCreationFailed = "OrderCreationFailed";
        public const string OrderCreationRollbackSucceed = "OrderCreationRollbackSucceed";
        public const string OrderCreationRollbackFailed = "OrderCreationRollbackFailed";
        public const string CatalogUpdated = "CatalogUpdated";
        public const string CatalogUpdateFailed = "CatalogUpdateFailed";
        public const string CatalogUpdateRollbackSucceed = "CatalogUpdateRollbackSucceed";
        public const string CatalogUpdateRollbackFailed = "CatalogUpdateRollbackFailed";
        public const string CustomerAmountUpdated = "CustomerAmountUpdated";
        public const string CustomerAmountUpdateFailed = "CustomerAmountUpdateFailed";
        public const string CustomerAmountUpdateRollbackSucceed = "CustomerAmountUpdateRollbackSucceed";
        public const string CustomerAmountUpdateRollbackFailed = "CustomerAmountUpdateRollbackFailed";
        public const string SagaSucceed = "SagaSucceed";
        public const string SagaFailed = "SagaFailed";
        public const string SagaUnexpectedError = "SagaUnexpectedError";
        public const string End = "End";
    }
}
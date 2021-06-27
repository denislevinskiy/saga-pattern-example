using Saga.Core.Extensions;

namespace Saga.Core.DTO.Error
{
    public sealed class UpdateCatalogErrorInfo : ErrorInfo
    {
        public string ToJsonString()
        {
            return JsonExtensions.SerializeObject(this);
        }
    }
}
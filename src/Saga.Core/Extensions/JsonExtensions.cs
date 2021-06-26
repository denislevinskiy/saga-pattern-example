using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Saga.Core.Extensions
{
    public static class JsonExtensions
    {
        public static string SerializeObject(object obj) =>
            JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            });

        public static T DeserializeObject<T>(string input) =>
            JsonConvert.DeserializeObject<T>(input, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            });
    }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Calories.App.Serialization
{
    public static class Serializer
    {
        private static JsonSerializerSettings _customSerializerSettings;
        private static JsonSerializerSettings CustomSerializerSettings
        {
            get
            {
                if (_customSerializerSettings == null)
                {
                    _customSerializerSettings = new JsonSerializerSettings();
                    _customSerializerSettings.Converters.Add(new StringEnumConverter());
                    _customSerializerSettings.Converters.Add(new JsonDateTimeConverter());
                    _customSerializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    };

                }

                return _customSerializerSettings;
            }
        }


        public static string ToJson(object objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize, CustomSerializerSettings);
        }

        public static TargetType FromJson<TargetType>(string json)
        {
            return JsonConvert.DeserializeObject<TargetType>(json, CustomSerializerSettings);
        }
    }
}

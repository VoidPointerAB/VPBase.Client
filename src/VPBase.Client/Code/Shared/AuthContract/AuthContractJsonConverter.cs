using VPBase.Auth.Contract.SharedInterfaces;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace VPBase.Client.Code.Shared.AuthContract
{
    public class AuthContractJsonConverter : IAuthContractJsonConverter
    {
        public T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public string SerializeObject(object value)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.SerializeObject(value, serializerSettings);
        }

        public string SerializeXmlNode(XmlNode node)
        {
            return JsonConvert.SerializeXmlNode(node);
        }
    }

    public class AuthContractJsonConverterFactory
    {
        public static IAuthContractJsonConverter Create()
        {
            return new AuthContractJsonConverter();
        }
    }
}
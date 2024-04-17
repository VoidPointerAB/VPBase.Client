using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VPBase.Auth.Client.SharedInterfaces;
using VPBase.Auth.Contract.SharedInterfaces;

namespace VPBase.Client.Code.Shared
{
    public class AuthClientHelper : IAuthClientHelper
    {
        private IAuthContractJsonConverter _jsonConverter;

        public AuthClientHelper(IAuthContractJsonConverter jsonConverter)
        {
            _jsonConverter = jsonConverter;
        }

        public string GetBasicAuthCredentials(string userName, string password)
        {
            var basicAuthCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + password));

            return basicAuthCredentials;
        }

        public IGraphQLVPBaseClient CreateGraphQLClient(string endPoint)
        {
            throw new Exception("GraphQL Client not implemented!");
        }

        public IRestVPBaseClient CreateRestClient()
        {
            var restClient = new HttpClient();

            return new RestVPBase3Client(restClient);
        }
    }

    public class RestVPBase3Client : IRestVPBaseClient
    {
        private HttpClient _httpClient;

        public RestVPBase3Client(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void AddDefaultRequestHeader(string name, string value)
        {
            _httpClient.DefaultRequestHeaders.Add(name, value);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
        {
            return _httpClient.SendAsync(request, completionOption);
        }
    }
}

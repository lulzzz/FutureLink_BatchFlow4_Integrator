using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace FutureLink.BatchFlow4.Integrator.API.Helpers
{
    public class ApiHelper
    {
        private static FutureLink.BatchFlow4.Integrator.API.Client _client { get; set; }
        public async static Task<FutureLink.BatchFlow4.Integrator.API.Client> GetAuthenticatedApiClient(string apiUrl, string serial)
        {

            HttpClient httpClient = new HttpClient();
            _client = new API.Client(apiUrl, httpClient);
            string token = await GetAuthToken(apiUrl, serial);
            _client.SetBearerToken(token);
            //httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            //_client = new Client(apiUrl, httpClient);
            return _client;

        }

        private static ObjectCache _authInfo = MemoryCache.Default;


        private async static Task<string> GetAuthToken(string apiUrl, string serial)
        {



                string appId = "BF4Integrator";
                string appSecret = "LetMeIn";
                string appClientName = "BF4 Integrator";


                HttpClient httpClient = new HttpClient();
                API.Client client = new API.Client(apiUrl, httpClient);

                var tokenResult = await client.AutheticateIntegrationClientsAsync(new APIAuthRequest() { ClientId = appId, ClientName = appClientName, ClientSecret = appSecret, Serial=serial });

                // subtracting 2 hours to compensate for server times being off-sync 
                return ((AuthToken)tokenResult).Access_token;
        }

    }
}

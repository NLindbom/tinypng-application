using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TinyPNGApplication.Client
{
    public class TinyPNGClient : IDisposable
    {
        private HttpClient client;
        private string apiKey;

        public string APIKey
        {
            get { return apiKey; }
            set
            {
                apiKey = value;
                Configure();
            }
        }

        public TinyPNGClient(string apiKey)
        {
            this.apiKey = apiKey;

            client = new HttpClient();

            Configure();
        }

        private void Configure()
        {
            const string authenticationHeaderValueScheme = "Basic";

            if (string.IsNullOrWhiteSpace(APIKey))
                return;

            var authenticationString = $"api:{APIKey}";

            var authenticationBytes = Encoding.UTF8.GetBytes(authenticationString);

            var authentication = Convert.ToBase64String(authenticationBytes);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(authenticationHeaderValueScheme, authentication);
        }

        public async Task<Response> ShrinkAsync(byte[] data)
        {
            const string shrinkEndpoint = "https://api.tinify.com/shrink";

            var responseMessage = await client.PostAsync(shrinkEndpoint, new ByteArrayContent(data));

            if (responseMessage.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return await Response.BuildResponseAsync<ShrinkResponse>(responseMessage);
            }

            return await Response.BuildResponseAsync<ErrorResponse>(responseMessage);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                client.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}

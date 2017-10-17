using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TinyPNGApplication.Client
{
    public abstract class Response
    {
        protected Response()
        {
            // Do not allow public creation of this object
        }

        public abstract Task BuildResponseAsync(HttpResponseMessage responseMessage);

        public static async Task<Response> BuildResponseAsync<T>(HttpResponseMessage responseMessage) where T : Response
        {
            var response = Activator.CreateInstance<T>();

            await response.BuildResponseAsync(responseMessage);

            return response;
        }
    }
}

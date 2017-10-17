using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TinyPNGApplication.Client
{
    public interface IResponseData
    {
    }

    public abstract class DataResponseBase<TResponseData> : Response where TResponseData : IResponseData
    {
        public TResponseData ResponseData { get; set; }

        public override async Task BuildResponseAsync(HttpResponseMessage responseMessage)
        {
            var contentString = await responseMessage.Content.ReadAsStringAsync();

            ResponseData = JsonConvert.DeserializeObject<TResponseData>(contentString);
        }
    }
}

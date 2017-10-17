using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TinyPNGApplication.Client
{
    public class ShrinkResponseData : IResponseData
    {
        public class InputData
        {
            public int Size { get; set; }
            public string Type { get; set; }
        }

        public InputData Input { get; set; }
    }

    public class ShrinkResponse : DataResponseBase<ShrinkResponseData>
    {
        public Uri Location { get; set; }

        public async override Task BuildResponseAsync(HttpResponseMessage responseMessage)
        {
            await base.BuildResponseAsync(responseMessage);

            Location = responseMessage.Headers.Location;
        }
    }
}

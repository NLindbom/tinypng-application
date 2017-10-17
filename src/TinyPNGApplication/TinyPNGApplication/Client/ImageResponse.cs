using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TinyPNGApplication.Client
{
    public class ImageResponse : Response
    {
        public string CompressionCount { get; set; }
        public string ImageWidth { get; set; }
        public string ImageHeight { get; set; }
        public string ContentType { get; set; }
        public string ContentLength{ get; set; }

        public byte[] Data { get; set; }

        public override async Task BuildResponseAsync(HttpResponseMessage responseMessage)
        {
            // TODO: headers

            Data = await responseMessage.Content.ReadAsByteArrayAsync();
        }
    }
}

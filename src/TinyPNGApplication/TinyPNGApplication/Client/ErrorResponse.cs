using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyPNGApplication.Client
{
    public class ErrorResponseData : IResponseData
    {
        public string Error { get; set; }
        public string Message { get; set; }
    }

    public class ErrorResponse : DataResponseBase<ErrorResponseData>
    {
    }
}

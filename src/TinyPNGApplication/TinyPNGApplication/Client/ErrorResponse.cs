using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyPNGApplication.Client
{
    public class ErrorResponse : IResponse
    {
        public string Error { get; set; }
        public string Message { get; set; }
    }
}

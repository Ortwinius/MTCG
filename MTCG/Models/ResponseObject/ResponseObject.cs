using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models.ResponseObject
{
    public class ResponseObject
    {
        public int StatusCode { get; set; }
        public string ResponseBody { get; set; }
        public ResponseObject(int statusCode, string responseBody)
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }
    }

}

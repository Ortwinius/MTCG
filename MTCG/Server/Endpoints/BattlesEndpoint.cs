using MTCG.Models.ResponseObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Server.Endpoints
{
    public class BattlesEndpoint : IHttpEndpoint
    {
        public ResponseObject HandleRequest(string method, string path, Dictionary<string,string> headers, string body)
        {
            throw new NotImplementedException();
        }
    }
}

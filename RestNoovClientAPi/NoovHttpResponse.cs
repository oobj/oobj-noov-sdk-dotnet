using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noov.Rest
{
   
    public class NoovHttpResponse
    {
        public String Content { get; set; }
        public int Status { get; set; }
        public Dictionary<String, String> Headers { get; set; }

        public NoovHttpResponse AddHeader(string key, string value)
        {
            if (Headers == null)
            {
                Headers = new Dictionary<string, string>();
            }
            Headers.Add(key, value);
            return this;
        }
    }
}

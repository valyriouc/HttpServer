using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerConsole;

internal class HttpHeaderDictionary : Dictionary<string, string>
{
    public HttpHeaderDictionary()
    {

    }

    public HttpHeaderDictionary(Dictionary<string, string> headers)
    {
        foreach (KeyValuePair<string, string> pair in headers)
        {
            this.Add(pair.Key, pair.Value);
        }
    }
}

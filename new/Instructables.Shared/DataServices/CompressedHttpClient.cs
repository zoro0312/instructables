using System.Net;
using System.Net.Http;

namespace Instructables.DataServices
{
    public class CompressedHttpClient : HttpClient
    {
        public CompressedHttpClient()
            : base(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip })
        {
        }

        public CompressedHttpClient(HttpClientHandler handler)
            : base(handler)
        {
        }
    }
}

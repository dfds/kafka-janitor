using Microsoft.Extensions.Configuration;

namespace Tika.Client
{
    public class TikaOptions
    {
        public string TIKA_API_ENDPOINT { get; set; }
        
        public TikaOptions() {}

        public TikaOptions(IConfiguration conf)
        {
            TIKA_API_ENDPOINT = conf["TIKA_API_ENDPOINT"];
        }
    }
}
using System;
using System.Net.Http;

namespace Tika.Client
{
    public class TikaClient
    {
        public ServiceAccounts ServiceAccounts { get; }
        public ApiKeys ApiKeys { get; }
        private readonly HttpClient _httpClient;

        private TikaClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            ServiceAccounts = new ServiceAccounts(_httpClient);
            ApiKeys = new ApiKeys(_httpClient);
        }

        public static TikaClient FromBaseUri(Uri uri)
        {
            return new TikaClient(new HttpClient {BaseAddress = uri});
        }
    }
}
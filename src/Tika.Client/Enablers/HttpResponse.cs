using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tika.Client.Enablers
{
    public static class HttpResponse
    {
        public static async Task<T> ToTypeAsync<T>(HttpResponseMessage httpResponseMessage)
        {
            httpResponseMessage.EnsureSuccessStatusCode();

            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            var deserializedObject = JsonConvert.DeserializeObject<T>(content);

            return deserializedObject;
        }
    }
} 
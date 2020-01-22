using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tika.Client.Models;

namespace Tika.Client.Enablers
{
    public class HttpRequest
    {
        public static async Task<T> Post<T>(
            HttpClient httpClient,
            Uri relativeUrl,
            object content
        )
        {
            var serializedContent = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = relativeUrl,
                Content = stringContent
            };

            var response = await httpClient.SendAsync(request);

            var deserializedObject = await HttpResponse.ToTypeAsync<T>(response);

            return deserializedObject;
        }
    }
}
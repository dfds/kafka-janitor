using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KafkaJanitor.IntegrationTests.Utils
{
    public class KafkaClient
    {
        public async Task SendMessageAsync(object message)
        {
            
            var httpClient = new HttpClient();


            var uri = new Uri("http://localhost:8082/topics/build.selfservice.events.topics");

            var messageInEnvelopes = PutInKafkaEnvelope(PutInOurEnvelope(message));
            
            var jsonSerializer = new JsonSerializer();
            var payload = jsonSerializer.Serialize(messageInEnvelopes);
            
            
            var content = new StringContent(
                payload, 
                Encoding.UTF8, 
                "application/vnd.kafka.json.v2+json"
            );

           var httpResponseMessage = await httpClient.PostAsync(uri, content);

           if (httpResponseMessage.IsSuccessStatusCode == false)
           {
               var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
               throw new Exception(message: 
                   $"Could not post message to Kafka. API response: {responseContent}" + Environment.NewLine + 
                   $"Payload: {payload}"
                );
           }
        }

        private dynamic PutInOurEnvelope(object payload)
        {
            return new ExternalEvent(
                version: "v1",
                eventName: "topic_added",
                xCorrelationId: "theFirstOne",
                xSender: "me",
                payload);
        }
        
        private dynamic PutInKafkaEnvelope(object payload)
        {
            
            var valueObj = new
            {
                value = payload
            };

            var recordsObj = new
            {
                records = new[] {valueObj}
            };
            
            return recordsObj;
        }
        
        private class ExternalEvent
        {

            public string Version { get; private set; }
            public string EventName { get; private set; }
            [JsonProperty(PropertyName = "x-correlationId")]
            public string XCorrelationId { get; private set; }
            [JsonProperty(PropertyName = "x-sender")]
            public string XSender { get; private set; }

            public object Payload { get; private set; }

            public ExternalEvent(
                string version, string eventName, string xCorrelationId, string xSender, object payload)
            {
                Version = version;
                EventName = eventName;
                XCorrelationId = xCorrelationId;
                XSender = xSender;
                Payload = payload;
            }
        }
    }
}
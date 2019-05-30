using System;
using Newtonsoft.Json.Linq;

namespace KafkaJanitor.WebApp.Infrastructure.Messaging
{
    public class MessageEmbeddedDocument
    {
        private readonly JObject _jObject;

        public MessageEmbeddedDocument(string json)
        {
            _jObject = JObject.Parse(json);
        }

//        public string MessageId => _jObject.SelectToken("messageId")?.Value<string>();
        public string EventName => _jObject.SelectToken("eventName")?.Value<string>();
        public string CorrelationId => _jObject.SelectToken("x-correlationId")?.Value<string>();

        public T ReadDataAs<T>() where T : class, new()
        {
            return (T) ReadDataAs(typeof(T));
        }

        public object ReadDataAs(Type messageInstanceType)
        {
            return _jObject
                .SelectToken("payload")
                .ToObject(messageInstanceType);
        }
    }
}
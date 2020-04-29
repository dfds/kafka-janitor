using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace KafkaClient
{
    public class JsonConsumer : IDisposable
    {
        public List<string> ConsumedMessages = new List<string>();
        private ConsumerBuilder<string, string> _consumerBuilder;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _token;

        public JsonConsumer(ConsumerBuilder<string, string> consumerBuilder)
        {
            _consumerBuilder = consumerBuilder;
            _cancellationTokenSource = new CancellationTokenSource();
            _token = _cancellationTokenSource.Token;
        }

        public void StartConsuming(string topicName)
        {
            Task.Run(() =>
                {
                    using (var consumer = _consumerBuilder.Build())
                    {
                        ConsumeResult<string, string> consumeResult;
                        consumer.Subscribe(topicName);
                        while (true)
                        {
                            consumeResult = consumer.Consume(_token);
                            ConsumedMessages.Add(consumeResult.Message.Value);

                            consumer.Commit(consumeResult);
                        } 
                    }
                },
                _token
            );
        }

        public T ConsumeOneOrDefault<T>()
        {
            var result = JsonConvert.DeserializeObject<T>(ConsumedMessages.First());
            ConsumedMessages.Remove(ConsumedMessages.First());
            return result;
        
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
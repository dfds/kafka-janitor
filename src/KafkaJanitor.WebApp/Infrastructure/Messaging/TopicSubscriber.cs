using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KafkaJanitor.WebApp.Infrastructure.Messaging
{
    public class TopicSubscriber : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IApplicationLifetime _applicationLifetime;

        public TopicSubscriber(IServiceProvider serviceProvider, IApplicationLifetime applicationLifetime)
        {
            _serviceProvider = serviceProvider;
            _applicationLifetime = applicationLifetime;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                try
                {
                    await StartSubscriberLoop(stoppingToken);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err);
                    _applicationLifetime.StopApplication();
                }
                
            }, stoppingToken);
        }

        private async Task StartSubscriberLoop(CancellationToken stoppingToken)
        {
            var kafkaConfiguration = _serviceProvider
                .GetRequiredService<KafkaConfiguration>()
                .GetConfiguration();

            using (var consumer = new ConsumerBuilder<string, string>(kafkaConfiguration).Build())
            {
                consumer.Subscribe("build.selfservice.events.topics");
                Console.WriteLine($"Now listening to topic(s): {string.Join(", ", consumer.Subscription)}");

                while (!stoppingToken.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    var message = new MessageEmbeddedDocument(consumeResult.Value);

                    await Handle(message);

                    consumer.Commit(consumeResult);
                }
            }
        }

        private async Task Handle(MessageEmbeddedDocument message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var messageHandler = scope.ServiceProvider.GetRequiredService<MessageHandler>();
                await messageHandler.Handle(message);
            }
        }
    }
}
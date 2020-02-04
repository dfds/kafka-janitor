using System;
using System.Threading.Tasks;
using KafkaJanitor.WebApp.Models;

namespace KafkaJanitor.WebApp.Infrastructure.Messaging
{
    public class MessageHandler
    {
        private readonly ITopicRepository _topicRepository;

        public MessageHandler(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }
        
        public Task Handle(MessageEmbeddedDocument message)
        {
            var eventName = message.EventName;
            
            switch (eventName)
            {
                case "topic_added":
                    var data = message.ReadDataAs<TopicAdded>();
                    return Handle(data);
                case "topic_created":
                    return HandleTopicCreated();
                
                default:
                    throw new Exception($"Unable to handle message {message.EventName}");
            }
        }

        private async Task Handle(TopicAdded message)
        {
            var alreadyExists = await _topicRepository.Exists(message.TopicName);

            if (alreadyExists)
            {
                return;
            }

            await _topicRepository.Add(new Topic(message.TopicName));
        }

        private async Task HandleTopicCreated()
        {
            // Do nothing as the payload sent by the new event is improper.
        }

        #region Message Definitions

        private class TopicAdded
        {
            public string TopicName { get; set; }
        }

        #endregion
    }
}
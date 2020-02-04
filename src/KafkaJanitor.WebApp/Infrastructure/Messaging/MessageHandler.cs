using System;
using System.Threading.Tasks;
using KafkaJanitor.WebApp.Models;
using Tika.Client;

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
                    {
                        var data = message.ReadDataAs<TopicAdded>();
                        return Handle(data);
                    }
                case "capablity_deleted":
                    {
                        var data = message.ReadDataAs<CapabilityDeleted>();
                        return HandleDeletionOfCapability(data);
                    }
                case "topic_creation_requested":
                {
                    var data = message.ReadDataAs<TopicCreationRequested>();
                    return HandleTopicCreationRequested(data);
                }
                case "topic_created":
                {
                    var data = message.ReadDataAs<TopicCreationRequested>();
                    return HandleTopicCreationRequested(data);
                }

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

        private async Task HandleTopicCreationRequested(TopicCreationRequested msg)
        {
            if (! await _topicRepository.Exists(msg.Name))
            {
                await _topicRepository.Add(new Topic(msg.Name, Convert.ToInt32(msg.Partitions)));
            }
            else
            {
                throw new Exception($"Topic '{msg.Name}' already exists");
            }
        }
        
        private async Task HandleDeletionOfCapability(CapabilityDeleted msg)
        {
            throw new NotImplementedException();
        }

        #region Message Definitions

        private class TopicAdded
        {
            public string TopicName { get; set; }
        }

        private class TopicCreationRequested
        {
            public string CapabilityId { get; set; }
            public string Name { get; set; }
            public string Partitions { get; set; }
            public string Description { get; set; }
        }

        private class CapabilityDeleted
        {
            public string CapabilityId { get; set; }
        }

        #endregion
    }
}
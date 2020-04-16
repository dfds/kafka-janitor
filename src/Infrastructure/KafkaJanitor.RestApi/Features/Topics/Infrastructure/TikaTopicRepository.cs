using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Domain;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;
using Tika.RestClient;
using Tika.RestClient.Features.Topics.Models;

namespace KafkaJanitor.RestApi.Features.Topics.Infrastructure
{
    public class TikaTopicRepository : ITopicRepository
    {
        private readonly IRestClient _tikaClient;

        public TikaTopicRepository(IRestClient tikaClient)
        {
            _tikaClient = tikaClient;
        }
        public async Task<IEnumerable<Topic>> GetAll()
        {
            var topics = await _tikaClient.Topics.GetAllAsync();
            return topics.Select(val => new Topic {Name = val});
        }


        public async Task<Topic> DescribeAsync(string topicName)
        {
            var topicDescription = await _tikaClient.Topics.DescribeAsync(topicName);

            var topic = new Topic
            {
                Name = topicDescription.name,
                Description = "",
                Partitions = topicDescription.partitionCount,
                Configurations = topicDescription.configurations
            };


            return topic;
        }

        public async Task Add(Topic topic)
        {
            var topicCreate = TopicCreate.Create(topic.Name, topic.Partitions);
            
            foreach (var (key, value) in topic.Configurations)
            {
                var jsonElement = (JsonElement)value;
                topicCreate = topicCreate.WithConfiguration(key, JsonObjectTools.GetValueFromJsonElement(jsonElement));
            }
            await _tikaClient.Topics.CreateAsync(topicCreate);
        }

        public async Task<bool> Exists(string topicName)
        {
            var topics = await _tikaClient.Topics.GetAllAsync();
            try
            {
                topics.First(to => to.Equals(topicName));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
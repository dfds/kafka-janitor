using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task Add(Topic topic)
        {
            await _tikaClient.Topics.CreateAsync(new TopicCreate
            {
                name = topic.Name,
                partitionCount = topic.Partitions
            });
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
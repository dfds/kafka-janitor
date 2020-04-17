using System;
using System.Collections.Generic;
using System.Text.Json;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;
using Xunit;

namespace KafkaJanitor.UnitTests.Features.Topics.Infrastructure
{
	public class JsonObjectToolsTest
    {
        private Topic _normalTopic;
        private Topic _deserializedTopic;
        private static string MAXSIZE = "maxSize";
        private static string RETENTIONLENGTH = "retentionLength";
        
        public JsonObjectToolsTest()
        {
	        _normalTopic = new Topic();
	        _normalTopic.Name = "testpelle-a01";
	        _normalTopic.Partitions = 3;
	        _normalTopic.Description = "A test Topic";
	        _normalTopic.Configurations = new Dictionary<string, object>();
	        _normalTopic.Configurations[MAXSIZE] = 3000;
	        _normalTopic.Configurations[RETENTIONLENGTH] = "3d";
	        
	        var serialized = JsonSerializer.Serialize(_normalTopic);
            _deserializedTopic = JsonSerializer.Deserialize<Topic>(serialized);
        }
        
        [Fact]
        public void PostConversionConfigurationsObjectsIsOfTypeJsonElement()
        {
            Assert.IsType<JsonElement>(_deserializedTopic.Configurations[MAXSIZE]);
            Assert.IsType<JsonElement>(_deserializedTopic.Configurations[RETENTIONLENGTH]);
            
            Assert.IsNotType<JsonElement>(_deserializedTopic.Name);
            Assert.IsNotType<JsonElement>(_deserializedTopic.Partitions);
            Assert.IsNotType<JsonElement>(_deserializedTopic.Description);
        }
        
        [Fact]
        public void CorrectTypesForValuesAreReturned()
        {
            var maxSize = (int)_normalTopic.Configurations[MAXSIZE];
            var expectedMaxSize = RestApi.Features.Topics.Infrastructure.JsonObjectTools.GetValueFromJsonElement((JsonElement)_deserializedTopic.Configurations[MAXSIZE]);
            var retentionLength = (String)_normalTopic.Configurations[RETENTIONLENGTH];
            var expectedRetentionLength = RestApi.Features.Topics.Infrastructure.JsonObjectTools.GetValueFromJsonElement((JsonElement)_deserializedTopic.Configurations[RETENTIONLENGTH]);

            
            Assert.Equal(maxSize, expectedMaxSize);
            Assert.Equal(retentionLength, expectedRetentionLength);
        }
    }
}
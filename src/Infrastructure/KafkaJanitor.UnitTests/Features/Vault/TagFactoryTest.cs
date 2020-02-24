using KafkaJanitor.RestApi.Features.Vault;
using Xunit;

namespace KafkaJanitor.UnitTests.Features.Vault
{
    public class TagFactoryTest
    {
        [Theory]
        [InlineData("thisis+very=much@okay", "  faaaaaaaaaa ::this.is/very-much_a=thing         ", "thisis+very=much@okay", "  faaaaaaaaaa ::this.is/very-much_a=thing         ")]
        [InlineData("!$£¼y", "x<>|*'", "y", "x")]
        public void OnlyAllowExpectedCharacters(string key, string value, string expectedKey, string expectedValue)
        {
            var tag = TagFactory.Create(key, value);
            
            Assert.Equal(expectedKey, tag.Key);
            Assert.Equal(expectedValue, tag.Value);
        }

        [Fact]
        public void TagKeyIsTruncatedIfMoreThan128Characters()
        {
            var tag = TagFactory.Create(
                "asdfsadfasdfasdfasdfadsfasdfsadfasdfasdfasdfadsfasdfsadfasdfasdfasdfadsfasdfsadfasdfasdfasdfadsfasdfsadfasdfasdfasdfadsfagfadsgdsfgsdfg",
                "x");
            
            Assert.Equal(128, tag.Key.Length);
        }

        [Fact]
        public void TagValueIsTruncatedIfMoreThan258Characters()
        {
            var tag = TagFactory.Create(
                "x",
                "asdfsadfasdfasdfasdfadsfasdfsadfasdfasdfasdfadsfasdfsadfasdfasdfasdfadsfasdfsadfasdfasdfasdfadsfasdfsadfasdfasdfasdfadsfagfadsgdsfgsdfgasdfsadfasdfasdfasdfadsfasdfsadfasdfasdfasdfadsfasdfsadfasdfasdfasdfadsfasdfsadfasdfasdfasdfadsfasdfsadfasdfasdfasdfadsfagfadsgdsfgsdfg");

            Assert.Equal(256, tag.Value.Length);

        }

        [Fact]
        public void OnlyIllegalCharsThrowsKeyLengthException()
        {
            Assert.Throws<TagKeyLengthException>(() =>
            {
                var tag = TagFactory.Create(
                    "!$£´¨'", "x");
            });
        }
        
        [Fact]
        public void OnlyIllegalCharsThrowsValueLengthException()
        {
            Assert.Throws<TagValueLengthException>(() =>
            {
                var tag = TagFactory.Create(
                    "x", "!");
            });
        }
    }
}
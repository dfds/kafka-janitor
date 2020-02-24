using System.Text.RegularExpressions;
using Amazon.SecurityToken.Model;

namespace KafkaJanitor.RestApi.Features.Vault
{
    public class TagFactory
    {
        public static Tag Create(string key, string value)
        {
            // https://docs.aws.amazon.com/general/latest/gr/aws_tagging.html
            var legalCharsKey = Regex.Replace(key, "[^A-Za-z0-9\\.:\\+=@_/-]+", "");
            var legalCharsValue = Regex.Replace(key, "[^A-Za-z0-9\\.:\\+=@_/-]+", "");

            var max128CharKey = legalCharsKey.Length <= 128 ? value : value.Substring(0, 128); 
            var max256CharValue = legalCharsValue.Length <= 258 ? value : value.Substring(0, 256);
            
            return new Tag{Key = max128CharKey, Value = max256CharValue};
        }
    }
}
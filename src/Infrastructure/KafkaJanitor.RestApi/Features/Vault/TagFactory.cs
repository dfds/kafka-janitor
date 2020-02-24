using System;
using System.Text.RegularExpressions;
using Amazon.SecurityToken.Model;

namespace KafkaJanitor.RestApi.Features.Vault
{
    public class TagFactory
    {
        public static Tag Create(string key, string value)
        {
            // https://docs.aws.amazon.com/general/latest/gr/aws_tagging.html
            var legalCharsKey = Regex.Replace(key, "[^A-Za-z0-9\\.: \\+=@_/-]+", "");
            var legalCharsValue = Regex.Replace(value, "[^A-Za-z0-9\\.: \\+=@_/-]+", "");

            var max128CharKey = legalCharsKey.Length <= 128 ? legalCharsKey : legalCharsKey.Substring(0, 128); 
            var max256CharValue = legalCharsValue.Length <= 256 ? legalCharsValue : legalCharsValue.Substring(0, 256);

            if (max128CharKey.Length == 0)
            {
                throw new TagKeyLengthException();
            }
            
            if (max256CharValue.Length == 0)
            {
                throw new TagValueLengthException();
            }
            
            return new Tag{Key = max128CharKey, Value = max256CharValue};
        }
    }

    public class TagKeyLengthException : Exception
    {
        public TagKeyLengthException() : base("Tag key is too short.") {}
    }
    
    public class TagValueLengthException : Exception
    {
        public TagValueLengthException() : base("Tag value is too short.") {}
    }
}
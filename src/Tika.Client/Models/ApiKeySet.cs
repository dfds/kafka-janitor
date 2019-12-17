namespace Tika.Client.Models
{
    public class ApiKeySet
    {
        public string Key { get; }
        public string Secret { get; }

        public ApiKeySet(
            string key, 
            string secret
        )
        {
            Key = key;
            Secret = secret;
        }
    }
}
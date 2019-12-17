namespace Tika.Client.Models
{
    public class ApiKey
    {
        public string Key { get; }
        public string Secret { get; }

        public ApiKey(
            string key, 
            string secret
        )
        {
            Key = key;
            Secret = secret;
        }
    }
}
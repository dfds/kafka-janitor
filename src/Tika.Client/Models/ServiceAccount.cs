namespace Tika.Client.Models
{
    public class ServiceAccount
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        public ServiceAccount(
            int id,
            string name,
            string description
        )
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
using System.Reflection;
using Dafda.Configuration;

namespace KafkaJanitor.App.Configurations.DomainEventAutoRegistrations;

public abstract class AutoDomainEventRegistrationBase
{
    private readonly IConfiguration _configuration;

    protected AutoDomainEventRegistrationBase(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected string GetTopicNameBy(string configurationKey)
        => _configuration[configurationKey] ?? throw new Exception($"Error! Missing topic configuration for key \"{configurationKey}\".");

    public virtual IEnumerable<ConsumerRegistration> GetConsumerRegistrations()
        => Enumerable.Empty<ConsumerRegistration>();

    public virtual IEnumerable<ProducerRegistration> GetProducerRegistrations()
        => Enumerable.Empty<ProducerRegistration>();

    public class ConsumerRegistration
    {
        private readonly List<Action<ConsumerOptions>> _registrations = new();

        public ConsumerRegistration(string groupId, Action<ConsumerOptions> registration)
        {
            GroupId = groupId;
            _registrations.Add(registration);
        }

        public string GroupId { get; }

        public void ApplyTo(ConsumerOptions options) => _registrations.ForEach(x => x(options));
    }

    public class ProducerRegistration
    {
        private readonly List<Action<OutboxOptions>> _registrations = new();

        public ProducerRegistration(Action<OutboxOptions> registration)
        {
            _registrations.Add(registration);
        }

        public void ApplyTo(OutboxOptions options) => _registrations.ForEach(x => x(options));
    }

    public static AutoDomainEventRegistrationBase[] DiscoverAndCreateAllRegistrators(IConfiguration configuration)
    {
        return Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(x => typeof(AutoDomainEventRegistrationBase).IsAssignableFrom(x))
            .Where(x => x.IsClass && !x.IsAbstract)
            .Select(x => (AutoDomainEventRegistrationBase)Activator.CreateInstance(x, configuration)!)
            .ToArray();

    }
}
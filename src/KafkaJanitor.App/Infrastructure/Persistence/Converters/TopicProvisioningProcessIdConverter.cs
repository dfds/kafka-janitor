using System.Linq.Expressions;
using KafkaJanitor.App.Domain.Model;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KafkaJanitor.App.Infrastructure.Persistence.Converters;

public class TopicProvisioningProcessIdConverter : ValueConverter<TopicProvisionProcessId, Guid>
{
    public TopicProvisioningProcessIdConverter() : base(ToDatabaseType, FromDatabaseType)
    {

    }

    private static Expression<Func<TopicProvisionProcessId, Guid>> ToDatabaseType => id => Guid.Parse(id.ToString());
    private static Expression<Func<Guid, TopicProvisionProcessId>> FromDatabaseType => value => TopicProvisionProcessId.Parse(value.ToString());
}
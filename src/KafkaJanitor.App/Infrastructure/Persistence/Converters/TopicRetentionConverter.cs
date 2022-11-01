using System.Linq.Expressions;
using KafkaJanitor.App.Domain.Model;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KafkaJanitor.App.Infrastructure.Persistence.Converters;

public class TopicRetentionConverter : ValueConverter<TopicRetention, uint>
{
    public TopicRetentionConverter() : base(ToDatabaseType, FromDatabaseType)
    {

    }

    private static Expression<Func<TopicRetention, uint>> ToDatabaseType => value => value.ToMilliseconds();
    private static Expression<Func<uint, TopicRetention>> FromDatabaseType => value => TopicRetention.FromMilliseconds(value);
}



public class AccessControlListEntryIdConverter : ValueConverter<AccessControlListEntryId, Guid>
{
    public AccessControlListEntryIdConverter() : base(ToDatabaseType, FromDatabaseType)
    {

    }

    private static Expression<Func<AccessControlListEntryId, Guid>> ToDatabaseType => value => value;
    private static Expression<Func<Guid, AccessControlListEntryId>> FromDatabaseType => value => value;
}

public class ClusterIdConverter : ValueConverter<ClusterId, string>
{
    public ClusterIdConverter() : base(ToDatabaseType, FromDatabaseType)
    {

    }

    private static Expression<Func<ClusterId, string>> ToDatabaseType => value => value.ToString();
    private static Expression<Func<string, ClusterId>> FromDatabaseType => value => ClusterId.Parse(value);
}
using System.Collections.Generic;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.Tests.Builders;

public class ClusterAccessDefinitionBuilder
{
    private ClusterAccessDefinitionId _id;
    private ClusterId _cluster;
    private ServiceAccountId _serviceAccount;
    private IEnumerable<AccessControlListEntry> _accessControlList;

    public ClusterAccessDefinitionBuilder()
    {
        _id = ClusterAccessDefinitionId.New();
        _cluster = A.ClusterId;
        _serviceAccount = A.ServiceAccountId;
        _accessControlList = new[]
        {
            A.AccessControlListEntry.Build()
        };
    }

    public ClusterAccessDefinition Build()
    {
        return new ClusterAccessDefinition(
            id: _id,
            cluster: _cluster,
            serviceAccount: _serviceAccount,
            accessControlList: _accessControlList
        );
    }
}
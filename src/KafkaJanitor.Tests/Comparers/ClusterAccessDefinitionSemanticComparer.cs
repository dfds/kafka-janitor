using System;
using System.Collections.Generic;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.Tests.Comparers;

public class ClusterAccessDefinitionSemanticComparer : IEqualityComparer<ClusterAccessDefinition?>
{
    public bool Equals(ClusterAccessDefinition? x, ClusterAccessDefinition? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (ReferenceEquals(x, null))
        {
            return false;
        }

        if (ReferenceEquals(y, null))
        {
            return false;
        }

        if (x.GetType() != y.GetType())
        {
            return false;
        }

        return x.ClusterId.Equals(y.ClusterId) &&
               x.ServiceAccountId.Equals(y.ServiceAccountId) &&
               x.AccessControlList.Equals(y.AccessControlList);
    }

    public int GetHashCode(ClusterAccessDefinition obj)
    {
        return HashCode.Combine(obj.ClusterId, obj.ServiceAccountId, obj.AccessControlList);
    }
}
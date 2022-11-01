using System;
using System.Collections.Generic;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.Tests.Comparers;

public class ServiceAccountSemanticComparer : IEqualityComparer<ServiceAccount?>
{
    public bool Equals(ServiceAccount? x, ServiceAccount? y)
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

        return x.CapabilityRootId.Equals(y.CapabilityRootId) && 
               x.AccessControlList.Equals(y.AccessControlList) &&
               x.ClusterApiKeys.Equals(y.ClusterApiKeys);
    }

    public int GetHashCode(ServiceAccount obj)
    {
        return HashCode.Combine(obj.CapabilityRootId, obj.AccessControlList, obj.ClusterApiKeys);
    }
}
using System;
using System.Collections.Generic;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.Tests.Comparers;

public class ClusterSemanticComparer : IEqualityComparer<Cluster?>
{
    public bool Equals(Cluster? x, Cluster? y)
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

        return x.Name == y.Name && 
               x.BootstrapEndpoint == y.BootstrapEndpoint;
    }

    public int GetHashCode(Cluster obj)
    {
        return HashCode.Combine(obj.Name, obj.BootstrapEndpoint);
    }
}
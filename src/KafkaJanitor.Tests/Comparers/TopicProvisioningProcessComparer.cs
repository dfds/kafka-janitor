using System;
using System.Collections.Generic;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.Tests.Comparers;

public class TopicProvisioningProcessComparer : IEqualityComparer<TopicProvisioningProcess?>
{
    public bool Equals(TopicProvisioningProcess? x, TopicProvisioningProcess? y)
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

        return x.RequestedTopic.Equals(y.RequestedTopic) &&
               x.ClusterId.Equals(y.ClusterId) &&
               x.Partitions.Equals(y.Partitions) &&
               x.Retention.Equals(y.Retention) &&
               x.IsServiceAccountCreated == y.IsServiceAccountCreated &&
               x.IsServiceAccountGrantedAccess == y.IsServiceAccountGrantedAccess &&
               x.IsTopicProvisioned == y.IsTopicProvisioned &&
               x.IsApiKeysCreated == y.IsApiKeysCreated &&
               x.AreAllApiKeysStoredInVault == y.AreAllApiKeysStoredInVault &&
               x.IsCompleted == y.IsCompleted;
    }

    public int GetHashCode(TopicProvisioningProcess obj)
    {
        var hashCode = new HashCode();
        hashCode.Add(obj.RequestedTopic);
        hashCode.Add(obj.ClusterId);
        hashCode.Add(obj.Partitions);
        hashCode.Add(obj.Retention);
        hashCode.Add(obj.IsServiceAccountCreated);
        hashCode.Add(obj.IsServiceAccountGrantedAccess);
        hashCode.Add(obj.IsTopicProvisioned);
        hashCode.Add(obj.IsApiKeysCreated);
        hashCode.Add(obj.AreAllApiKeysStoredInVault);
        hashCode.Add(obj.IsCompleted);
        return hashCode.ToHashCode();
    }
}
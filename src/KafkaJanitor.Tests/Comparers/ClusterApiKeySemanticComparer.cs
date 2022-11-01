using System;
using System.Collections.Generic;
using KafkaJanitor.App.Domain.Model;

namespace KafkaJanitor.Tests.Comparers;

public class ClusterApiKeySemanticComparer : IComparer<ClusterApiKey?>
{
    public int Compare(ClusterApiKey? x, ClusterApiKey? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        if (ReferenceEquals(null, y))
        {
            return 1;
        }

        if (ReferenceEquals(null, x))
        {
            return -1;
        }

        var userNameComparison = string.Compare(x.UserName, y.UserName, StringComparison.Ordinal);
        if (userNameComparison != 0)
        {
            return userNameComparison;
        }

        var passwordComparison = string.Compare(x.Password, y.Password, StringComparison.Ordinal);
        if (passwordComparison != 0)
        {
            return passwordComparison;
        }

        var passwordChecksumComparison = string.Compare(x.PasswordChecksum, y.PasswordChecksum, StringComparison.Ordinal);
        if (passwordChecksumComparison != 0)
        {
            return passwordChecksumComparison;
        }

        return x.IsStoredInVault.CompareTo(y.IsStoredInVault);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Tika.RestClient.Features.Acls.Models;

namespace KafkaJanitor.RestApi.Features.AccessControlLists.Domain.Models
{
    public static class AccessControlLists
    {
          private const string PUB_DOT = "pub.";

        public static int AclTemplateCount => GetAllAcls("0", "").Count();
    
        public static IEnumerable<AclCreateDelete> GetAllAcls(string serviceAccountId, string prefix)
        {
            var serviceAccountIdAsInt = Convert.ToInt64(serviceAccountId);

            var allAcls =
                GlobalAcls(serviceAccountIdAsInt)
                    .Concat(TopicsAcls(serviceAccountIdAsInt, prefix))
                    .Concat(ConsumerGroupAcls(serviceAccountIdAsInt, prefix));

            return allAcls;
        }

        public static AclCreateDelete[] ConsumerGroupAcls(long serviceAccountIdAsInt, string prefix)
        {
            return new[]
            {
                // For root-id.*
                new AclCreateDelete(serviceAccountIdAsInt, true, "WRITE", "", prefix),
                new AclCreateDelete(serviceAccountIdAsInt, true, "CREATE", "", prefix),
                new AclCreateDelete(serviceAccountIdAsInt, true, "READ", "", prefix),
                    
                // For connect-rootid.*
                new AclCreateDelete(serviceAccountIdAsInt, true, "WRITE", "", $"connect-{prefix}"),
                new AclCreateDelete(serviceAccountIdAsInt, true, "CREATE", "", $"connect-{prefix}"),
                new AclCreateDelete(serviceAccountIdAsInt, true, "READ", "", $"connect-{prefix}")
            };
        }

        public static AclCreateDelete[] TopicsAcls(long serviceAccountIdAsInt, string prefix)
        {
            return new[]
            {
                new AclCreateDelete(serviceAccountIdAsInt, true, "WRITE", $"{PUB_DOT}{prefix}."),
                new AclCreateDelete(serviceAccountIdAsInt, true, "CREATE", $"{PUB_DOT}{prefix}."),
                new AclCreateDelete(serviceAccountIdAsInt, true, "WRITE", prefix),
                new AclCreateDelete(serviceAccountIdAsInt, true, "CREATE", prefix),
                new AclCreateDelete(serviceAccountIdAsInt, true, "READ", prefix),
                new AclCreateDelete(serviceAccountIdAsInt, true, "DESCRIBE", prefix),
                new AclCreateDelete(serviceAccountIdAsInt, true, "DESCRIBE-CONFIGS", prefix)
            };
        }

        public static AclCreateDelete[] GlobalAcls(long serviceAccountIdAsInt)
        {
            return new[]
            {
                new AclCreateDelete(serviceAccountIdAsInt, false, "alter"),
                new AclCreateDelete(serviceAccountIdAsInt, false, "alter-configs"),
                new AclCreateDelete(serviceAccountIdAsInt, false, "cluster-action"),
                new AclCreateDelete(serviceAccountIdAsInt, false, "create", "'*'"),
                new AclCreateDelete(serviceAccountIdAsInt, true, "READ", PUB_DOT)
            };
        }
    }
}

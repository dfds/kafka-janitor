using System;
using System.Collections.Generic;
using System.Linq;
using Tika.RestClient.Features.Acls.Models;

namespace KafkaJanitor.RestApi.Features.AccessControlLists.Domain.Models
{
    public static class AccessControlLists
    {
        public static int AclTemplateCount => GetAllAcls("0", "").Count();
    
        public static IEnumerable<AclCreateDelete> GetAllAcls(string serviceAccountId, string prefix)
        {
            var allAcls =
                GlobalAcls(serviceAccountId)
                    .Concat(TopicsAcls(serviceAccountId, prefix))
                    .Concat(ConsumerGroupAcls(serviceAccountId, prefix));

            return allAcls;
        }

        public static AclCreateDelete[] ConsumerGroupAcls(string serviceAccountId, string prefix)
        {
            return new[]
            {
                // For root-id.*
                new AclCreateDelete(serviceAccountId, true, "WRITE", "", prefix),
                new AclCreateDelete(serviceAccountId, true, "CREATE", "", prefix),
                new AclCreateDelete(serviceAccountId, true, "READ", "", prefix),
                    
                // For connect-rootid.*
                new AclCreateDelete(serviceAccountId, true, "WRITE", "", $"connect-{prefix}"),
                new AclCreateDelete(serviceAccountId, true, "CREATE", "", $"connect-{prefix}"),
                new AclCreateDelete(serviceAccountId, true, "READ", "", $"connect-{prefix}")
            };
        }

        public static AclCreateDelete[] TopicsAcls(string serviceAccountId, string prefix)
        {
            return new[]
            {
                new AclCreateDelete(serviceAccountId, true, "WRITE", $"pub.{prefix}."),
                new AclCreateDelete(serviceAccountId, true, "CREATE", $"pub.{prefix}."),
                new AclCreateDelete(serviceAccountId, true, "WRITE", prefix),
                new AclCreateDelete(serviceAccountId, true, "CREATE", prefix),
                new AclCreateDelete(serviceAccountId, true, "READ", prefix),
                new AclCreateDelete(serviceAccountId, true, "DESCRIBE", prefix),
                new AclCreateDelete(serviceAccountId, true, "DESCRIBE-CONFIGS", prefix)
            };
        }

        public static AclCreateDelete[] GlobalAcls(string serviceAccountId)
        {
            return new[]
            {
                new AclCreateDelete(serviceAccountId, false, "alter"),
                new AclCreateDelete(serviceAccountId, false, "alter-configs"),
                new AclCreateDelete(serviceAccountId, false, "cluster-action"),
                new AclCreateDelete(serviceAccountId, false, "create", "'*'"),
                new AclCreateDelete(serviceAccountId, true, "READ", "pub.")
            };
        }
    }
}

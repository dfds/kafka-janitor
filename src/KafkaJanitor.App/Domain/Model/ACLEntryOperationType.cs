namespace KafkaJanitor.App.Domain.Model;

public enum ACLEntryOperationType
{
    Create,
    Read,
    Write,
    Describe,
    DescribeConfigs,
    Alter,
    AlterConfigs,
    ClusterAction,
}
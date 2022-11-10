-- 2022-11-10 14:39:29 : add-table-for-topic-provisioning-process

CREATE TABLE public."TopicProvisioningProcess" (
    "Id" uuid NOT NULL,
    "RequestedTopic" varchar(4096) NOT NULL,
    "ClusterId" varchar(50) NOT NULL,
    "Partitions" int NOT NULL,
    "Retention" int NOT NULL,
    "CapabilityRootId" varchar(1024) NOT NULL,
    "IsServiceAccountCreated" boolean NOT NULL,
    "IsServiceAccountGrantedAccessToCluster" boolean NOT NULL,
    "IsTopicProvisioned" boolean NOT NULL,
    "IsApiKeyStoredInVault" boolean NOT NULL,
    "IsCompleted" boolean NOT NULL,
    CONSTRAINT "pk_topicprovisioningprocess" PRIMARY KEY ("Id")
);

CREATE INDEX "ix_topicprovisioningprocess_capabilityrootid" ON public."TopicProvisioningProcess" ("CapabilityRootId");
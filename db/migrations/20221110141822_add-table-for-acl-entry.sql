-- 2022-11-10 14:18:22 : add-table-for-acl-entry

CREATE TABLE public."AccessControlListEntry" (
    "Id" uuid NOT NULL,
    "IsApplied" boolean NOT NULL,
    "Descriptor_ResourceType" varchar(100) NOT NULL,
    "Descriptor_ResourceName" varchar(1024) NOT NULL,
    "Descriptor_PatternType" varchar(100) NOT NULL,
    "Descriptor_OperationType" varchar(100) NOT NULL,
    "Descriptor_PermissionType" varchar(100) NOT NULL,
    "ClusterAccessDefinitionId" uuid NULL,
    CONSTRAINT "pk_accesscontrollistentry" PRIMARY KEY ("Id")
);

CREATE INDEX "ix_accesscontrollistentry_clusteraccessdefinitionid" ON public."AccessControlListEntry" ("ClusterAccessDefinitionId");
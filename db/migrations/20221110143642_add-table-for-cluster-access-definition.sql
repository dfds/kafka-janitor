-- 2022-11-10 14:36:42 : add-table-for-cluster-access-definition

CREATE TABLE public."ClusterAccessDefinition" (
    "Id" uuid NOT NULL,
    "ClusterId" varchar(50) NOT NULL,
    "ServiceAccountId" varchar(50) NOT NULL,
    CONSTRAINT "pk_clusteraccessdefinition" PRIMARY KEY ("Id")
);

CREATE INDEX "ix_clusteraccessdefinition_serviceaccountid" ON public."ClusterAccessDefinition" ("ServiceAccountId");
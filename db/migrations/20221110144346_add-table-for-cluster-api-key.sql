-- 2022-11-10 14:43:46 : add-table-for-cluster-api-key

CREATE TABLE public."ClusterApiKey" (
    "Id" uuid NOT NULL,
    "UserName" text NOT NULL,
    "Password" text NOT NULL,
    "PasswordChecksum" text NOT NULL,
    "ClusterId" varchar(50) NOT NULL,
    "IsStoredInVault" boolean NOT NULL,
    "ServiceAccountId" varchar(50) NULL,
    CONSTRAINT "pk_clusterapikey" PRIMARY KEY ("Id")
);

CREATE INDEX "ix_clusterapikey_serviceaccountid" ON public."ClusterApiKey" ("ServiceAccountId");
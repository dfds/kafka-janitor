-- 2022-11-10 14:10:05 : add-table-for-service-account

CREATE TABLE public."ServiceAccount" (
    "Id" varchar(50) NOT NULL,
    "CapabilityRootId" varchar(1024) NOT NULL,
    CONSTRAINT "pk_serviceaccount" PRIMARY KEY ("Id")
);

CREATE INDEX "ix_serviceaccount_capabilityrootid" ON public."ServiceAccount" ("CapabilityRootId");
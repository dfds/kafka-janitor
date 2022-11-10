-- 2022-11-10 14:35:01 : add-table-for-cluster

CREATE TABLE public."Cluster" (
    "Id" varchar(50) NOT NULL,
    "Name" varchar(1024) NOT NULL,
    "BootstrapEndpoint" varchar(4096) NOT NULL,
    "AdminApiEndpoint" varchar(4096) NOT NULL,
    CONSTRAINT "pk_cluster" PRIMARY KEY ("Id")
);
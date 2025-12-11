-- LIMS Migration Engine DDL (SQL Server)
-- 1. Mapping table
CREATE TABLE dbo.LimsColumnMapping (
    MappingId        INT IDENTITY(1,1) PRIMARY KEY,
    SourceSystem     NVARCHAR(100) NOT NULL,
    SourceTableName  NVARCHAR(128) NOT NULL,
    SourceColumnName NVARCHAR(128) NOT NULL,
    SourceDataType   NVARCHAR(100) NULL,
    DestinationSystem NVARCHAR(100) NOT NULL,
    DestinationTableName NVARCHAR(128) NOT NULL,
    DestinationColumnName NVARCHAR(128) NOT NULL,
    DestinationDataType NVARCHAR(100) NULL,
    IsKeyField       BIT NOT NULL DEFAULT 0,
    TransformationRule NVARCHAR(MAX) NULL,
    IsActive         BIT NOT NULL DEFAULT 1,
    MappingGroup     NVARCHAR(100) NULL,
    CreatedOn        DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy        NVARCHAR(100) NULL
);
CREATE INDEX IX_LimsColumnMapping_Group ON dbo.LimsColumnMapping(MappingGroup);
CREATE INDEX IX_LimsColumnMapping_Source ON dbo.LimsColumnMapping(SourceSystem, SourceTableName);

-- 2. Staging extract table
CREATE TABLE dbo.StagingExtract (
    StagingId        BIGINT IDENTITY(1,1) PRIMARY KEY,
    SourceSystem     NVARCHAR(100) NOT NULL,
    SourceTableName  NVARCHAR(128) NOT NULL,
    ExtractedJson    NVARCHAR(MAX) NOT NULL,
    ExtractedOn      DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
CREATE INDEX IX_StagingExtract_Source ON dbo.StagingExtract(SourceSystem, SourceTableName);

-- 3. MigrationTransaction
CREATE TABLE dbo.MigrationTransaction (
    TransactionId    BIGINT IDENTITY(1,1) PRIMARY KEY,
    QueryName        NVARCHAR(200) NOT NULL,
    MappingGroup     NVARCHAR(100) NULL,
    DestinationTable NVARCHAR(128) NULL,
    GeneratedQuery   NVARCHAR(MAX) NOT NULL,
    ExecutionStatus  NVARCHAR(50) NOT NULL,
    ErrorMessage     NVARCHAR(MAX) NULL,
    RecordsAffected  BIGINT NULL,
    StartedOn        DATETIME2 NULL,
    CompletedOn      DATETIME2 NULL,
    CreatedBy        NVARCHAR(100) NULL
);
CREATE INDEX IX_MigrationTransaction_Status ON dbo.MigrationTransaction(ExecutionStatus);

-- 4. ReferenceValue sample table
CREATE TABLE dbo.ReferenceValue (
    RefId INT IDENTITY(1,1) PRIMARY KEY,
    RefName NVARCHAR(100) NOT NULL,
    SourceValue NVARCHAR(200) NOT NULL,
    DestValue NVARCHAR(200) NOT NULL
);

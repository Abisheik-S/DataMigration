LIMS Migration Engine - Package
================================

Contents
--------
- sql/ddl.sql              : Database DDL for mapping, staging, transactions
- src/Models              : C# domain models (LimsColumnMapping, MigrationTransaction, StagingExtract)
- src/Repositories       : Repository interfaces and Dapper-based implementations (skeletons)
- src/Services           : DynamicSqlBuilder, MigrationService (orchestration)
- docs/architecture.md   : Mermaid diagram + PNG image of architecture
- package.zip            : This full package (created automatically)

How to use
----------
1. Run the SQL in `sql/ddl.sql` against your destination SQL Server database.
2. Copy the C# files into an ASP.NET Core project. The code uses Dapper and IDbConnection.
3. Wire dependencies in Startup/Program:
   - IDbConnection for your DB
   - register repositories and services
4. Seed mapping rows in `dbo.LimsColumnMapping` and call the API endpoint:
   POST /api/v1/migration/run/{mappingGroup}

Notes
-----
- The DynamicSqlBuilder uses a `{col}` placeholder in TransformationRule to substitute source column expressions.
- For production, use two DB connections (source read, destination write), implement batching, validation, and secure mapping edits.

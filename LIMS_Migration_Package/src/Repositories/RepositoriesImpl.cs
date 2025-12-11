using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using LimsMigration.Models;

namespace LimsMigration.Repositories
{
    public class LimsMappingRepository : ILimsMappingRepository
    {
        private readonly IDbConnection _db;
        public LimsMappingRepository(IDbConnection db) { _db = db; }

        public async Task<IEnumerable<LimsColumnMapping>> GetActiveMappingsByGroupAsync(string mappingGroup)
        {
            var sql = "SELECT * FROM dbo.LimsColumnMapping WHERE IsActive = 1 AND MappingGroup = @group ORDER BY MappingId";
            return await _db.QueryAsync<LimsColumnMapping>(sql, new { group = mappingGroup });
        }

        public async Task<IEnumerable<LimsColumnMapping>> GetActiveMappingsBySourceAsync(string sourceSystem, string sourceTable)
        {
            var sql = "SELECT * FROM dbo.LimsColumnMapping WHERE IsActive = 1 AND SourceSystem = @src AND SourceTableName = @table";
            return await _db.QueryAsync<LimsColumnMapping>(sql, new { src = sourceSystem, table = sourceTable });
        }
    }

    public class MigrationTransactionRepository : IMigrationTransactionRepository
    {
        private readonly IDbConnection _db;
        public MigrationTransactionRepository(IDbConnection db) { _db = db; }

        public async Task<long> CreateTransactionAsync(MigrationTransaction txn)
        {
            var sql = @"
INSERT INTO dbo.MigrationTransaction
(QueryName, MappingGroup, DestinationTable, GeneratedQuery, ExecutionStatus, CreatedBy)
VALUES (@QueryName, @MappingGroup, @DestinationTable, @GeneratedQuery, @ExecutionStatus, @CreatedBy);
SELECT CAST(SCOPE_IDENTITY() as bigint);";
            var id = await _db.QuerySingleAsync<long>(sql, txn);
            return id;
        }

        public async Task UpdateTransactionAsync(MigrationTransaction txn)
        {
            var sql = @"UPDATE dbo.MigrationTransaction
SET ExecutionStatus = @ExecutionStatus,
    ErrorMessage = @ErrorMessage,
    RecordsAffected = @RecordsAffected,
    StartedOn = @StartedOn,
    CompletedOn = @CompletedOn
WHERE TransactionId = @TransactionId";
            await _db.ExecuteAsync(sql, txn);
        }
    }
}

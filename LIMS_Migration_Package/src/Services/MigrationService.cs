using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using LimsMigration.Models;
using LimsMigration.Repositories;

namespace LimsMigration.Services
{
    public class MigrationService
    {
        private readonly ILimsMappingRepository _mappingRepo;
        private readonly IMigrationTransactionRepository _txRepo;
        private readonly IDbConnection _db;
        private readonly DynamicSqlBuilder _sqlBuilder;

        public MigrationService(ILimsMappingRepository mappingRepo,
                                IMigrationTransactionRepository txRepo,
                                IDbConnection db,
                                DynamicSqlBuilder sqlBuilder)
        {
            _mappingRepo = mappingRepo;
            _txRepo = txRepo;
            _db = db;
            _sqlBuilder = sqlBuilder;
        }

        public async Task<MigrationTransaction> RunMigrationGroupAsync(string mappingGroup, string createdBy = "system")
        {
            var mappings = (await _mappingRepo.GetActiveMappingsByGroupAsync(mappingGroup)).ToList();
            if (!mappings.Any())
                throw new InvalidOperationException($"No mappings found for group {mappingGroup}");

            MigrationTransaction lastTxn = null;
            var byDest = mappings.GroupBy(m => m.DestinationTableName);

            foreach (var grp in byDest)
            {
                var mapList = grp.ToList();
                var sql = _sqlBuilder.BuildInsertSelectQuery(mapList);

                var txn = new MigrationTransaction
                {
                    QueryName = $"Migrate_{grp.Key}_{mappingGroup}_{DateTime.UtcNow:yyyyMMddHHmmss}",
                    MappingGroup = mappingGroup,
                    DestinationTable = grp.Key,
                    GeneratedQuery = sql,
                    ExecutionStatus = "Pending",
                    CreatedBy = createdBy
                };

                var txnId = await _txRepo.CreateTransactionAsync(txn);
                txn.TransactionId = txnId;

                try
                {
                    txn.ExecutionStatus = "Running";
                    txn.StartedOn = DateTime.UtcNow;
                    await _txRepo.UpdateTransactionAsync(txn);

                    var rows = await _db.ExecuteAsync(sql);

                    txn.ExecutionStatus = "Success";
                    txn.RecordsAffected = rows;
                    txn.CompletedOn = DateTime.UtcNow;
                    txn.ErrorMessage = null;

                    await _txRepo.UpdateTransactionAsync(txn);
                }
                catch (Exception ex)
                {
                    txn.ExecutionStatus = "Failed";
                    txn.ErrorMessage = ex.ToString();
                    txn.CompletedOn = DateTime.UtcNow;
                    await _txRepo.UpdateTransactionAsync(txn);
                }

                lastTxn = txn;
            }

            return lastTxn;
        }
    }
}

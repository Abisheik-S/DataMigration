using System.Collections.Generic;
using System.Threading.Tasks;
using LimsMigration.Models;

namespace LimsMigration.Repositories
{
    public interface ILimsMappingRepository
    {
        Task<IEnumerable<LimsColumnMapping>> GetActiveMappingsByGroupAsync(string mappingGroup);
        Task<IEnumerable<LimsColumnMapping>> GetActiveMappingsBySourceAsync(string sourceSystem, string sourceTable);
    }

    public interface IMigrationTransactionRepository
    {
        Task<long> CreateTransactionAsync(MigrationTransaction txn);
        Task UpdateTransactionAsync(MigrationTransaction txn);
    }
}

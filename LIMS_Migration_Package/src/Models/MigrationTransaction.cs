using System;

namespace LimsMigration.Models
{
    public class MigrationTransaction
    {
        public long TransactionId { get; set; }
        public string QueryName { get; set; }
        public string MappingGroup { get; set; }
        public string DestinationTable { get; set; }
        public string GeneratedQuery { get; set; }
        public string ExecutionStatus { get; set; }
        public string ErrorMessage { get; set; }
        public long? RecordsAffected { get; set; }
        public DateTime? StartedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}

using System;

namespace LimsMigration.Models
{
    public class StagingExtract
    {
        public long StagingId { get; set; }
        public string SourceSystem { get; set; }
        public string SourceTableName { get; set; }
        public string ExtractedJson { get; set; }
        public DateTime ExtractedOn { get; set; }
    }
}

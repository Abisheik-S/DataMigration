using System;

namespace LimsMigration.Models
{
    public class LimsColumnMapping
    {
        public int MappingId { get; set; }
        public string SourceSystem { get; set; }
        public string SourceTableName { get; set; }
        public string SourceColumnName { get; set; }
        public string SourceDataType { get; set; }
        public string DestinationSystem { get; set; }
        public string DestinationTableName { get; set; }
        public string DestinationColumnName { get; set; }
        public string DestinationDataType { get; set; }
        public bool IsKeyField { get; set; }
        public string TransformationRule { get; set; }
        public bool IsActive { get; set; }
        public string MappingGroup { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

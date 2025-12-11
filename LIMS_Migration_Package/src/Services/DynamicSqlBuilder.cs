using System;
using System.Collections.Generic;
using System.Linq;
using LimsMigration.Models;

namespace LimsMigration.Services
{
    public class DynamicSqlBuilder
    {
        public string BuildInsertSelectQuery(IEnumerable<LimsColumnMapping> mappings)
        {
            if (mappings == null || !mappings.Any()) throw new ArgumentException("mappings empty");

            var mapList = mappings.ToList();
            var destTable = mapList.First().DestinationTableName;
            var sourceTable = mapList.First().SourceTableName;
            var sourceAlias = "src";

            var destCols = mapList.Select(m => $"[{m.DestinationColumnName}]").ToList();

            var selectExprs = mapList.Select(m =>
            {
                var srcColRef = $"{sourceAlias}.[{m.SourceColumnName}]";
                if (!string.IsNullOrWhiteSpace(m.TransformationRule))
                {
                    return m.TransformationRule.Replace("{col}", srcColRef) + $" AS [{m.DestinationColumnName}]";
                }
                return srcColRef + $" AS [{m.DestinationColumnName}]";
            }).ToList();

            var insertCols = string.Join(", ", destCols);
            var selectCols = string.Join(",
    ", selectExprs);

            var sql = $@"
INSERT INTO {destTable} ({insertCols})
SELECT
    {selectCols}
FROM {sourceTable} {sourceAlias};";

            return sql;
        }
    }
}

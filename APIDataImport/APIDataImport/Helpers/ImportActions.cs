//=================================================================================================================================================================================||
//---------------------------------------------------------------------------- MODIFICATION LOG------------------------------------------------------------------------------------||
// DATE        |      CODE          |                              DETAIL                                                                                                          ||
// 31-JUN-2023 |       -            |    Initial version         - Code Refactorization, Optimisation and Cleanup                                                                  ||
//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------||
//=================================================================================================================================================================================||


using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Thermo.SampleManager.Common.Data;
using Thermo.SampleManager.ObjectModel;

namespace WEBApiRequestTask.ALS_Data_Import.Helpers
{
    
    public class ImportActions
    {
        public static Action<List<IEntityCollection>, List<Dictionary<string, LabAliasEntry>>>
            SetupAliases = (LabAliasCollection, HashTableCollection) =>
            {
                for (int i = 0; i < LabAliasCollection.Count; i++)
                {
                    foreach (LabAliasEntry mEntry in LabAliasCollection[i])
                    {
                        InitializeHashTables(mEntry.Input, mEntry, HashTableCollection[i]);
                    }
                }
            };
      

        public static Action<string, LabAliasEntry, Dictionary<string, LabAliasEntry>> InitializeHashTables = (key, value, HashTable) =>
        {
            if (!HashTable.ContainsKey(key?.Trim()))
            {
                HashTable.Add(key?.Trim(), value);
            }
        };

        
        public static Action<string, string, Dictionary<string, string>> UpdateHashTable = (key, item, HashTable) =>
        {
            if (!HashTable.ContainsKey(key))
            {
                HashTable.Add(key, item);
            }
        };

        [ItemCanBeNull]
        public static Func<LabAlias, string, LabAliasEntry> GetAliasValue = (LabAlias labAlias, string input) =>
        {
            var labAliasEntry = labAlias.LabAliasEntries
            .ActiveItems
            .Cast<LabAliasEntry>()
            .Where(x => x.Input == input);

            return (LabAliasEntry)labAliasEntry;
        };


        [CanBeNull]
        public static Func<string, LabAlias, IEntityManager, LabAliasEntry> GetAliasLike = (string input, LabAlias aliasEntity, IEntityManager _EntityManager) =>
        {
            IQuery aliasQuery = _EntityManager.CreateQuery<LabAliasEntry>();
            aliasQuery.AddEquals(LabAliasEntryPropertyNames.LabAliasId, aliasEntity.LabAliasId);
            aliasQuery.AddEquals(LabAliasEntryPropertyNames.LabAliasVersion, aliasEntity.LabAliasVersion);
            aliasQuery.AddLike(LabAliasEntryPropertyNames.Input, $"%{input}%");
            var collection = _EntityManager.Select(aliasQuery);

            if (collection.ActiveCount == 0)
            {

                return null;
            }
            else
            {
                return collection.ActiveItems.FirstOrDefault() as LabAliasEntry;
            }

        };

        [CanBeNull]
        public static Func<string, dynamic, dynamic> GetValueFromDictionary = (string key, dynamic dictionary) =>
        {
            dictionary.TryGetValue(key, out dynamic value);
            return value;
        };
    }
}

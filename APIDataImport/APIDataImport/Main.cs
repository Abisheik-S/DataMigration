// Decompiled with JetBrains decompiler
// Type: APIDataImport.Main
// Assembly: APIDataImport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F18B3FE-91D4-415D-BCFE-CA5883100F9A
// Assembly location: C:\Users\Abisheik.S\Documents\Hudbay\APIDataImport.dll

using APIDataImport.DataImport;
using APIDataImport.FileReader;
using APIDataImport.Helpers;
using APIDataImport.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using Thermo.SampleManager.Common.CommandLine;
using Thermo.SampleManager.Common.Data;
using Thermo.SampleManager.Library;
using Thermo.SampleManager.Library.EntityDefinition;
using Thermo.SampleManager.ObjectModel;

namespace APIDataImport
{
  [SampleManagerTask("FileDataImport", "WorkflowCallback")]
  public class Main : SampleManagerTask, IBackgroundTask
  {
    public string FolderPath = (string) null;
    public LabAlias SampleAlias = (LabAlias) null;
    public LabAlias ResultAlias = (LabAlias) null;
    public LabAlias SampleTypeAlias = (LabAlias) null;
    public List<Sample> SampleLocalList = new List<Sample>();
    public Dictionary<string, LabAliasEntry> ResultAliasEntries = new Dictionary<string, LabAliasEntry>();
    public Dictionary<string, LabAliasEntry> SampleAliasEntries = new Dictionary<string, LabAliasEntry>();
    public Dictionary<string, LabAliasEntry> SampleTypeAliasEntries = new Dictionary<string, LabAliasEntry>();

    public void Launch()
    {
    }

    protected virtual void SetupTask()
    {
      if (this.Library.Environment.IsBackground())
      {
        this.Driver();
      }
      else
      {
        try
        {
          this.Library.Utils.SetBusy(this.Library.MessageGroups.SchemaMessages, "Loading", "Loading");
          this.Driver();
          this.Library.Utils.FlashMessage("Data Import Completed", "Success");
        }
        finally
        {
          this.Library.Utils.ClearBusy();
        }
      }
      ConfigHeader configHeader = this.EntityManager.Select("CONFIG_HEADER", new Identity(new object[1]
      {
        (object) ConfigItems.WebApiTokenConfig
      })) as ConfigHeader;
      ((ConfigHeaderBase) configHeader).Value = string.Empty;
      this.EntityManager.Transaction.Add((IEntity) configHeader);
      this.Logger.Error((object) "Proceso Terminado Lectura Archivos API");
      this.Exit((object) true);
    }

    private void Driver()
    {
      this.InitializeAliasEntities();
      this.Logger.Error((object) "Inicio Proceso Lectura Archivos API");
      this.FolderPath = this.Library.Environment.GetGlobalString(ConfigItems.FilePathConfig);
      foreach (string enumerateFile in Directory.EnumerateFiles(this.FolderPath, "*.txt"))
        FileParser.DeconstructFile(enumerateFile, this.FolderPath);
      this.ProcessSampleFile();
      this.ProcessResultFile();
    }

    private void ProcessResultFile()
    {
      foreach (string enumerateFile in Directory.EnumerateFiles(this.FolderPath + "Temp", "Resultados*.txt"))
      {
        if (string.IsNullOrEmpty(File.ReadAllText(enumerateFile)?.Trim()))
        {
          File.Delete(enumerateFile);
        }
        else
        {
          this.Logger.Error((object) ("Procesando Resultados en Archivo Temp " + enumerateFile + "..."));
          new ResultFileDataImport((IDataImport<IEntity>) new ResultDataImport(this.Logger, this.Library, this.EntityManager)).ProcessFile((object) enumerateFile, (object) this.ResultAliasEntries);
          File.Delete(enumerateFile);
          this.Logger.Error((object) "Proceso de Resultados Terminado");
        }
      }
    }

    private void ProcessSampleFile()
    {
      foreach (string enumerateFile in Directory.EnumerateFiles(this.FolderPath + "Temp", "Muestras*.txt"))
      {
        if (string.IsNullOrEmpty(File.ReadAllText(enumerateFile)?.Trim()))
        {
          File.Delete(enumerateFile);
        }
        else
        {
          this.Logger.Error((object) ("Procesando Muestras en Archivo Temp " + enumerateFile + "..."));
          new SampleFileDataImport((IDataImport<IEntity>) new SampleDataImport(this.Logger, this.Library, this.EntityManager)).ProcessFile((object) enumerateFile, (object) this.SampleAlias, (object) this.SampleAliasEntries, (object) this.SampleTypeAliasEntries);
          File.Delete(enumerateFile);
          this.Logger.Error((object) "Proceso de Muestras Terminado");
        }
      }
    }

    private void InitializeAliasEntities()
    {
      IQuery query1 = this.EntityManager.CreateQuery("LAB_ALIAS");
      query1.AddEquals("LabAliasName", (object) ConfigItems.LabAliasSample);
      IQuery query2 = this.EntityManager.CreateQuery("LAB_ALIAS");
      query2.AddEquals("LabAliasName", (object) ConfigItems.LabAliasResult);
      IQuery query3 = this.EntityManager.CreateQuery("LAB_ALIAS");
      query3.AddEquals("LabAliasName", (object) ConfigItems.LabSampleTypeAlias);
      this.SampleAlias = (LabAlias) this.EntityManager.Select(query1, true).GetFirst();
      this.ResultAlias = (LabAlias) this.EntityManager.Select(query2, true).GetFirst();
      this.SampleTypeAlias = (LabAlias) this.EntityManager.Select(query3, true).GetFirst();
      Action<List<IEntityCollection>, List<Dictionary<string, LabAliasEntry>>> setupAliases = ImportActions.SetupAliases;
      List<IEntityCollection> ientityCollectionList = new List<IEntityCollection>();
      ientityCollectionList.Add(((LabAliasBase) this.SampleAlias).LabAliasEntries);
      ientityCollectionList.Add(((LabAliasBase) this.ResultAlias).LabAliasEntries);
      ientityCollectionList.Add(((LabAliasBase) this.SampleTypeAlias).LabAliasEntries);
      List<Dictionary<string, LabAliasEntry>> dictionaryList = new List<Dictionary<string, LabAliasEntry>>();
      dictionaryList.Add(this.SampleAliasEntries);
      dictionaryList.Add(this.ResultAliasEntries);
      dictionaryList.Add(this.SampleTypeAliasEntries);
      setupAliases(ientityCollectionList, dictionaryList);
    }
  }
}

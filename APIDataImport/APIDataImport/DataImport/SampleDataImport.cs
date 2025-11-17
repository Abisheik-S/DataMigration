// Decompiled with JetBrains decompiler
// Type: APIDataImport.DataImport.SampleDataImport
// Assembly: APIDataImport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F18B3FE-91D4-415D-BCFE-CA5883100F9A
// Assembly location: C:\Users\Abisheik.S\Documents\Hudbay\APIDataImport.dll

using APIDataImport.Helpers;
using APIDataImport.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using Thermo.Framework.Core;
using Thermo.SampleManager.Common.Data;
using Thermo.SampleManager.Common.Workflow;
using Thermo.SampleManager.Library;
using Thermo.SampleManager.Library.DesignerRuntime;
using Thermo.SampleManager.Library.EntityDefinition;
using Thermo.SampleManager.ObjectModel;
using Thermo.SampleManager.Server.Workflow.Definition;

namespace APIDataImport.DataImport
{
  public class SampleDataImport : IDataImport<IEntity>
  {
    public StandardLibrary ImLibrary { get; set; }

    public IEntityManager ImEntityManager { get; set; }

    public Logger ImLogger { get; set; }

    private Thermo.SampleManager.ObjectModel.Workflow SampleLoginWorkflow { get; set; }

    public SampleDataImport(Logger logger, StandardLibrary library, IEntityManager entityManager)
    {
      this.ImLogger = logger;
      this.ImLibrary = library;
      this.ImEntityManager = entityManager;
      this.SampleLoginWorkflow = this.ImEntityManager.SelectLatestVersion("WORKFLOW", this.ImLibrary.Environment.GetGlobalString(ConfigItems.SampleWorkflowConfig), true, true) as Thermo.SampleManager.ObjectModel.Workflow;
    }

    public IEntity Create(params object[] properties)
    {
      LabAliasEntry property1 = properties[0] as LabAliasEntry;
      WEBMuestras property2 = properties[1] as WEBMuestras;
      LabAliasEntry property3 = properties[2] as LabAliasEntry;
      Sample sample = this.RunWorkflow(this.SampleLoginWorkflow, "SAMPLE") as Sample;
      ((SampleBase) sample).SampleName = property2.ID_MUESTRA.ToString();
      ((SampleBase) sample).LoginDate = NullableDateTime.op_Implicit(DateTime.Parse(property2.FECHA_MUESTREO));
      ((SampleBase) sample).SampledDate = NullableDateTime.op_Implicit(DateTime.Parse(property2.FECHA_MUESTREO));
      ((SampleBase) sample).DateCompleted = NullableDateTime.op_Implicit(DateTime.Parse(property2.FECHA_EMISION_INFORME));
      ((SampleBase) sample).ProductLink = (MlpHeaderBase) this.ImEntityManager.SelectLatestVersion("MLP_HEADER", ((LabAliasEntryBase) property1).Output, false, true);
      ((SampleBase) sample).SampleType = (PhraseBase) (this.ImEntityManager.SelectPhrase("SAMP_TYPE", ((LabAliasEntryBase) property3).Output) as Phrase);
      ((SampleBase) sample).SamplingPoint = (SamplePointBase) this.ImEntityManager.SelectByName<SamplePoint>(property2.NOMBRE_ESTACION);
      if (((BaseEntity) ((SampleBase) sample).SamplingPoint).IsNull() || ((BaseEntity) ((SampleBase) sample).ProductLink).IsNull() || ((BaseEntity) ((SampleBase) sample).SampleType).IsNull())
      {
        this.ImLogger.Error((object) ("Null Values found for the sample " + ((SampleBase) sample).SampleName + ", SamplePointName " + property2.NOMBRE_ESTACION + " . Sample creation aborted"));
        return (IEntity) null;
      }
      this.ImEntityManager.Transaction.Add((IEntity) sample);
      return (IEntity) sample;
    }

    public IEntity Update(params object[] properties)
    {
      Sample property1 = properties[0] as Sample;
      LabAliasEntry property2 = properties[1] as LabAliasEntry;
      LabAliasEntry property3 = properties[2] as LabAliasEntry;
      WEBMuestras property4 = properties[3] as WEBMuestras;
      ((SampleBase) property1).LoginDate = NullableDateTime.op_Implicit(DateTime.Parse(property4.FECHA_MUESTREO));
      ((SampleBase) property1).SampledDate = NullableDateTime.op_Implicit(DateTime.Parse(property4.FECHA_MUESTREO));
      ((SampleBase) property1).DateCompleted = NullableDateTime.op_Implicit(DateTime.Parse(property4.FECHA_EMISION_INFORME));
      ((SampleBase) property1).ProductLink = (MlpHeaderBase) this.ImEntityManager.SelectLatestVersion("MLP_HEADER", ((LabAliasEntryBase) property2).Output, false, true);
      ((SampleBase) property1).SampleType = (PhraseBase) (this.ImEntityManager.SelectPhrase("SAMP_TYPE", ((LabAliasEntryBase) property3).Output) as Phrase);
      ((SampleBase) property1).SamplingPoint = (SamplePointBase) this.ImEntityManager.SelectByName<SamplePoint>(property4.NOMBRE_ESTACION);
      this.ImLogger.Debug((object) ("Sample update started " + string.Format("Sample- {0}", (object) ((SampleBase) property1).IdNumeric) + string.Format("Sampled_date- {0},", (object) ((SampleBase) property1).SampledDate) + string.Format("DateCompleted- {0},", (object) ((SampleBase) property1).DateCompleted) + string.Format("Product- {0},", (object) ((SampleBase) property1).ProductLink) + string.Format("SampleType- {0},", (object) ((SampleBase) property1).SampleType) + string.Format("Sample_point- {0}", (object) ((SampleBase) property1).SamplingPoint)));
      if (((BaseEntity) ((SampleBase) property1).SamplingPoint).IsNull())
        this.ImLogger.Error((object) ("Null Values found for the sample " + ((SampleBase) property1).SampleName + ", SamplePointName " + property4.NOMBRE_ESTACION + ", Product - " + ((LabAliasEntryBase) property2).Output));
      if (((BaseEntity) ((SampleBase) property1).ProductLink).IsNull())
        this.ImLogger.Error((object) ("Null Values found for the sample " + ((SampleBase) property1).SampleName + ", Product - " + ((LabAliasEntryBase) property2).Output));
      this.ImLogger.Error((object) string.Format("Sample update completed for sample {0}", (object) ((SampleBase) property1).IdNumeric));
      return (IEntity) property1;
    }

    public void Commit() => this.ImEntityManager.Commit();

    public void Log(string value) => this.ImLogger.Error((object) value);

    public bool IsComponentsValid(LabAlias sampleAlias) => this.IsComponentsValid(this.SampleLoginWorkflow, sampleAlias);

    public bool IsComponentsValid(Thermo.SampleManager.ObjectModel.Workflow sampleLoginWorkflow, LabAlias sampleAlias)
    {
      bool flag = true;
      if (sampleAlias == null)
      {
        this.ImLogger.Error((object) ("Alias [" + ConfigItems.LabAliasSample + "] not found"));
        flag = false;
      }
      if (sampleLoginWorkflow == null)
      {
        this.ImLogger.Error((object) ("Workflow [" + ConfigItems.SampleWorkflowConfig + "] not found"));
        flag = false;
      }
      return flag;
    }

    public bool IsSampleNew(
      LabAliasEntry samplelabAliasEntry,
      WEBMuestras muestra,
      ref Sample mSample)
    {
      if (samplelabAliasEntry == null)
        return false;
      IQuery query = this.ImEntityManager.CreateQuery<Sample>();
      query.AddEquals("SAMPLE_NAME", (object) muestra.ID_MUESTRA.ToString());
      query.AddEquals("SampledDate", (object) DateTime.Parse(muestra.FECHA_MUESTREO));
      query.AddEquals("Product", (object) ((LabAliasEntryBase) samplelabAliasEntry).Output);
      IEntityCollection ientityCollection = this.ImEntityManager.Select(query, true);
      if (((ICollection) ientityCollection).Count <= 0)
        return true;
      this.ImLogger.Error((object) string.Format("Muestra Existe {0}", (object) ((SampleBase) ientityCollection.GetFirst()).IdNumeric));
      mSample = ientityCollection.GetFirst() as Sample;
      return false;
    }

    private IEntity RunWorkflow(Thermo.SampleManager.ObjectModel.Workflow workFlow, string entityType)
    {
      IWorkflowPropertyBag iworkflowPropertyBag = ((WorkflowInternal) workFlow).Perform();
      return ((List<WorkflowError>) iworkflowPropertyBag.Errors).Count == 0 ? iworkflowPropertyBag.GetEntities(entityType)[0] : (IEntity) null;
    }
  }
}

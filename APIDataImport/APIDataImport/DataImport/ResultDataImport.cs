// Decompiled with JetBrains decompiler
// Type: APIDataImport.DataImport.ResultDataImport
// Assembly: APIDataImport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F18B3FE-91D4-415D-BCFE-CA5883100F9A
// Assembly location: C:\Users\Abisheik.S\Documents\Hudbay\APIDataImport.dll

using APIDataImport.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Thermo.Framework.Core;
using Thermo.SampleManager.Common.Data;
using Thermo.SampleManager.Internal.ObjectModel;
using Thermo.SampleManager.Library.DesignerRuntime;
using Thermo.SampleManager.Library.EntityDefinition;
using Thermo.SampleManager.ObjectModel;

namespace APIDataImport.DataImport
{
  internal class ResultDataImport : IDataImport<IEntity>
  {
    private readonly StandardLibrary _Library;
    private readonly IEntityManager _EntityManager;
    private readonly Logger _Logger;
    private readonly Dictionary<int, object[]> newTestKeyPair;

    public ResultDataImport(Logger logger, StandardLibrary library, IEntityManager entityManager)
    {
      this._Logger = logger;
      this._Library = library;
      this._EntityManager = entityManager;
      this.newTestKeyPair = new Dictionary<int, object[]>();
    }

    public void Commit() => this._EntityManager.Commit();

    public bool TransferResult(params object[] properties)
    {
      VersionedAnalysis versionedAnalysis = (VersionedAnalysis) this._EntityManager.SelectLatestVersion("VERSIONED_ANALYSIS", ((LabAliasEntryBase) (properties[2] as LabAliasEntry)).Output, false, true);
      if (versionedAnalysis == null)
        return false;
      return this.Create(new object[3]
      {
        (object) versionedAnalysis,
        properties[0],
        properties[1]
      }) != null;
    }

    public IEntity Create(params object[] properties)
    {
      VersionedAnalysis property1 = properties[0] as VersionedAnalysis;
      Sample property2 = properties[1] as Sample;
      WEBResultados property3 = properties[2] as WEBResultados;
      if (property2.IsTestAssigned(((VersionedAnalysisBase) property1).VersionedAnalysisName))
      {
        Test test = (Test) ((SampleInternal) property2).GetTest((VersionedAnalysisInternal) property1);
        if (((ICollection) ((TestBase) test).Results).Count <= 0)
          return (IEntity) test;
        return this.Update(new object[3]
        {
          (object) property1,
          (object) property2,
          (object) property3
        });
      }
      Test test1 = ((IEnumerable) ((SampleInternal) property2).AddTest((VersionedAnalysisInternal) property1, true)).Cast<Test>().FirstOrDefault<Test>();
      this._Logger.Error((object) string.Format("Adding new test ={0} for sample{1}", (object) property1, (object) ((SampleBase) property2).IdNumeric));
      this._EntityManager.Transaction.Add((IEntity) test1);
      if (!(((VersionedAnalysisBase) property1).Components.GetFirst() is VersionedComponent first))
      {
        this._Logger.Error((object) string.Format("No componenets found for the analysis ={0} for sample{1}", (object) property1, (object) ((SampleBase) property2).IdNumeric));
        return (IEntity) null;
      }
      object[] objArray = new object[3]
      {
        (object) test1,
        (object) ((VersionedComponentBase) first).VersionedComponentName,
        (object) property3.RESULTADO
      };
      if (this.newTestKeyPair.ContainsKey(((SampleBase) property2).IdNumeric.Value))
        this.newTestKeyPair[((SampleBase) property2).IdNumeric.Value] = objArray;
      else
        this.newTestKeyPair.Add(((SampleBase) property2).IdNumeric.Value, objArray);
      return (IEntity) test1;
    }

    public IEntity Update(params object[] properties)
    {
      VersionedAnalysis analysis = properties[0] as VersionedAnalysis;
      Sample property1 = properties[1] as Sample;
      WEBResultados property2 = properties[2] as WEBResultados;
      Test property3 = properties[3] as Test;
      Result result = ((IEnumerable) ((TestBase) property3).Results.ActiveItems).Cast<Result>().Where<Result>((Func<Result, bool>) (x => ((ResultBase) x).ResultName == ((VersionedComponentBase) (((VersionedAnalysisBase) analysis).Components.GetFirst() as VersionedComponent)).VersionedComponentName)).FirstOrDefault<Result>();
      if (((ResultBase) result).Text.Equals(property2.RESULTADO))
        return (IEntity) null;
      this._Logger.Error((object) string.Format("Adding existing results for sample = {0}, test = {1}, component = {2}, result={3}", (object) ((SampleBase) property1).IdNumeric, (object) ((TestBase) property3).TestNumber, (object) ((ResultBase) result).ResultName, (object) property2.RESULTADO));
      this.EnterResultVGL(((TestBase) property3).TestNumber.Value, ((VersionedComponentBase) ((IEnumerable) ((VersionedAnalysisBase) analysis).Components).Cast<VersionedComponent>().FirstOrDefault<VersionedComponent>()).VersionedComponentName, property2.RESULTADO, ((SampleBase) property1).IdNumeric.Value);
      return (IEntity) property3;
    }

    public void Log(string value) => this._Logger.Error((object) value);

    public void ProcessNewResult()
    {
      foreach (KeyValuePair<int, object[]> keyValuePair in this.newTestKeyPair)
      {
        int key = keyValuePair.Key;
        int test = ((TestBase) (keyValuePair.Value[0] as Test)).TestNumber.Value;
        string component = keyValuePair.Value[1].ToString();
        string result = keyValuePair.Value[2].ToString();
        this._Logger.Error((object) string.Format("Adding new results for sample = {0}, test = {1}, component = {2}, result={3}", (object) key, (object) test, (object) component, (object) result));
        this.EnterResultVGL(test, component, result, key);
      }
    }

    public void EnterResultVGL(int test, string component, string result, int sample) => this._Library.VGL.RunVGLRoutine("API_UTILS", "save_result", new object[4]
    {
      (object) test,
      (object) component,
      (object) result,
      (object) sample
    });

    public Sample CheckSampleExists(string value)
    {
      IQuery query = this._EntityManager.CreateQuery<Sample>();
      query.AddEquals("SAMPLE_NAME", (object) value);
      return this._EntityManager.Select(query, true).GetFirst() as Sample;
    }
  }
}

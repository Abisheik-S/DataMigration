// Decompiled with JetBrains decompiler
// Type: WEBApiRequestTask.ALS_Data_Import.Misc.RunDataFix
// Assembly: APIDataImport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F18B3FE-91D4-415D-BCFE-CA5883100F9A
// Assembly location: C:\Users\Abisheik.S\Documents\Hudbay\APIDataImport.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Thermo.SampleManager.Common.CommandLine;
using Thermo.SampleManager.Common.Data;
using Thermo.SampleManager.Internal.ObjectModel;
using Thermo.SampleManager.Library;
using Thermo.SampleManager.Library.EntityDefinition;
using Thermo.SampleManager.ObjectModel;

namespace WEBApiRequestTask.ALS_Data_Import.Misc
{
  [SampleManagerTask("RunDataFix")]
  public class RunDataFix : SampleManagerTask, IBackgroundTask
  {
    public int ResultCount = 0;

    [CommandLineSwitch("FileName", "FileName of the Inputfile")]
    public string CommandFileName { get; set; }

    private void SetupTaskBackground()
    {
      try
      {
        string commandFileName = this.CommandFileName;
        this.Logger.Error((object) ("Processing File " + this.CommandFileName));
        string[] strArray1 = File.ReadAllLines(new FileInfo(commandFileName).FullName);
        List<DataModel> dataModelList = new List<DataModel>();
        for (int index = 0; index < strArray1.Length; ++index)
        {
          if (!string.IsNullOrEmpty(strArray1[index]))
          {
            string[] strArray2 = strArray1[index].Split(',');
            dataModelList.Add(new DataModel()
            {
              Id = strArray2[0],
              Component = strArray2[1],
              Value = strArray2[2]
            });
          }
        }
        this.Logger.Error((object) "Converted CSV lines into Objects");
        foreach (DataModel dm in dataModelList)
        {
          IQuery query1 = this.EntityManager.CreateQuery<Sample>();
          query1.AddEquals("IdNumeric", (object) dm.Id);
          if (this.EntityManager.Select(query1, true).GetFirst() is Sample first1)
          {
            this.Logger.Error((object) string.Format("Fetching sample {0}", (object) ((SampleBase) first1).IdNumeric));
            IQuery query2 = this.EntityManager.CreateQuery<VersionedAnalysis>();
            query2.AddEquals("Identity", (object) dm.Component);
            query2.AddOrder("AnalysisVersion", false);
            if (!(this.EntityManager.Select(query2, true).GetFirst() is VersionedAnalysis first))
            {
              this.Logger.Error((object) ("analysis is null " + dm.Component));
            }
            else
            {
              try
              {
                if (!first1.IsTestAssigned(((VersionedAnalysisBase) first).VersionedAnalysisName))
                {
                  this.CreateNewTest(first, first1, dm);
                  ++this.ResultCount;
                }
                else
                  this.UpdateExistingTest(first, first1, dm);
                this.EntityManager.Commit();
              }
              catch (Exception ex)
              {
                this.Logger.Error((object) (ex.Message + ex.StackTrace));
                continue;
              }
              this.EntityManager.Transaction.Add((IEntity) first1);
              this.EntityManager.Commit();
            }
          }
        }
        this.Logger.Error((object) "Result Import is Completed");
        this.Logger.Error((object) string.Format("Completed File {0} with result count {1}", (object) this.CommandFileName, (object) this.ResultCount));
      }
      finally
      {
        this.EntityManager.Commit();
      }
    }

    private void UpdateExistingTest(VersionedAnalysis analysis, Sample msample, DataModel dm)
    {
      this.Logger.Error((object) string.Format("Updating test {0} for the sample {1}", (object) analysis, (object) msample));
      foreach (TestBase testBase in ((IEnumerable<IEntity>) ((SampleBase) msample).Tests.ActiveItems).Where<IEntity>((Func<IEntity, bool>) (x => ((TestBase) (x as Test)).Analysis.Identity.Equals(((VersionedAnalysisBase) analysis).Identity))).ToList<IEntity>())
      {
        if (((ResultBase) (testBase.Results.GetFirst() as Result)).Text == dm.Value)
        {
          this.Logger.Error((object) string.Format("update skipped to test {0} for sample {1} due to redundancy", (object) analysis, (object) msample));
        }
        else
        {
          ++this.ResultCount;
          this.Logger.Error((object) string.Format("Adding replicate test to  {0} for sample {1} ", (object) analysis, (object) msample));
          Test test = ((IEnumerable<TestInternal>) ((SampleInternal) msample).AddTest((VersionedAnalysisInternal) analysis, true)).FirstOrDefault<TestInternal>() as Test;
          string s = dm.Value;
          if (dm.Value.Contains("<") || dm.Value.Contains(">"))
            s = dm.Value.Replace('>', ' ').Replace('<', ' ').Trim().Replace(' ', '.');
          ResultBase resultBase = ((TestInternal) test).AddResult(((VersionedComponentBase) (((VersionedAnalysisBase) analysis).Components.GetFirst() as VersionedComponent)).VersionedComponentName);
          double result;
          bool flag = double.TryParse(s, out result);
          if (dm.Value.Contains<char>('-'))
            s = s.Replace('-', ',');
          resultBase.Value = flag ? result : 0.0;
          resultBase.Text = dm.Value;
          resultBase.RawResult = s;
          resultBase.EnteredOn = ((SampleBase) msample).DateCompleted;
          resultBase.EnteredBy = ((PersonnelBase) this.Library.Environment.CurrentUser).Identity.Trim();
          this.EntityManager.Transaction.Add((IEntity) test);
          this.EntityManager.Transaction.Add((IEntity) resultBase);
        }
      }
    }

    private void CreateNewTest(VersionedAnalysis analysis, Sample msample, DataModel dm)
    {
      this.Logger.Error((object) string.Format("adding test {0}", (object) analysis));
      TestInternal testInternal = ((IEnumerable<TestInternal>) ((SampleInternal) msample).AddTest((VersionedAnalysisInternal) analysis, true)).FirstOrDefault<TestInternal>();
      string s = dm.Value;
      if (dm.Value.Contains("<") || dm.Value.Contains(">"))
        s = dm.Value.Replace('>', ' ').Replace('<', ' ').Trim().Replace(' ', '.');
      ResultBase resultBase = testInternal.AddResult(((VersionedComponentBase) (((VersionedAnalysisBase) analysis).Components.GetFirst() as VersionedComponent)).VersionedComponentName);
      double result;
      bool flag = double.TryParse(s, out result);
      if (dm.Value.Contains<char>('-'))
        s = s.Replace('-', ',');
      resultBase.Value = flag ? result : 0.0;
      resultBase.Text = dm.Value;
      resultBase.RawResult = s;
      resultBase.EnteredOn = ((SampleBase) msample).DateCompleted;
      resultBase.EnteredBy = ((PersonnelBase) this.Library.Environment.CurrentUser).Identity.Trim();
      this.EntityManager.Transaction.Add((IEntity) testInternal);
      this.EntityManager.Transaction.Add((IEntity) resultBase);
    }

    public void Launch() => this.SetupTaskBackground();
  }
}

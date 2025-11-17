// Decompiled with JetBrains decompiler
// Type: WEBApiRequestTask.ALS_Data_Import.Misc.ResultModel
// Assembly: APIDataImport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F18B3FE-91D4-415D-BCFE-CA5883100F9A
// Assembly location: C:\Users\Abisheik.S\Documents\Hudbay\APIDataImport.dll

using Thermo.SampleManager.Internal.ObjectModel;
using Thermo.SampleManager.Library.EntityDefinition;
using Thermo.SampleManager.ObjectModel;

namespace WEBApiRequestTask.ALS_Data_Import.Misc
{
  public class ResultModel
  {
    public Sample SampleEntity;
    public TestInternal TestEntity;
    public string Analysis;
    public ResultBase Result;
    public string Value;
  }
}

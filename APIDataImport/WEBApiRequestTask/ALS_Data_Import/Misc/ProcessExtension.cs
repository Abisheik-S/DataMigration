// Decompiled with JetBrains decompiler
// Type: WEBApiRequestTask.ALS_Data_Import.Misc.ProcessExtension
// Assembly: APIDataImport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F18B3FE-91D4-415D-BCFE-CA5883100F9A
// Assembly location: C:\Users\Abisheik.S\Documents\Hudbay\APIDataImport.dll

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WEBApiRequestTask.ALS_Data_Import.Misc
{
  public static class ProcessExtension
  {
    public static Task WaitForExitAsyncExt(this Process process) => Task.Run((Action) (() => process.WaitForExit()));
  }
}

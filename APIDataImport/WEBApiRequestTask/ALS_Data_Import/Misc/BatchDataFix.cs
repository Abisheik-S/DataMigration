// Decompiled with JetBrains decompiler
// Type: WEBApiRequestTask.ALS_Data_Import.Misc.BatchDataFix
// Assembly: APIDataImport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F18B3FE-91D4-415D-BCFE-CA5883100F9A
// Assembly location: C:\Users\Abisheik.S\Documents\Hudbay\APIDataImport.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Thermo.SampleManager.Library;

namespace WEBApiRequestTask.ALS_Data_Import.Misc
{
  [SampleManagerTask("BatchDataImportTq")]
  public class BatchDataFix : SampleManagerTask
  {
    private readonly int poolSize = 10000;
    private readonly string folderName = "C:\\Users\\bmadmin\\Desktop\\Import";
    private readonly List<Task> processTasks = new List<Task>();

    protected virtual void SetupTask()
    {
      this.AsyncDriver().Wait();
      this.EntityManager.Commit();
    }

    public async Task AsyncDriver()
    {
      try
      {
        string[] strArray = Directory.GetFiles(this.folderName);
        for (int index = 0; index < strArray.Length; ++index)
        {
          string file = strArray[index];
          int maxLength = File.ReadAllLines(file).Length;
          if (maxLength < 1)
            return;
          FileInfo fileInfo = new FileInfo(file);
          IEnumerable<string> sourceArray = File.ReadLines(file);
          for (int i = 0; i < maxLength; i += this.poolSize)
          {
            string newFilename = fileInfo.DirectoryName + "\\batches\\" + fileInfo.Name + "_" + i.ToString() + ".csv";
            IEnumerable<string> batchArray = sourceArray.Skip<string>(i).Take<string>(this.poolSize);
            File.AppendAllLines(newFilename, batchArray);
            newFilename = (string) null;
            batchArray = (IEnumerable<string>) null;
          }
          fileInfo = (FileInfo) null;
          sourceArray = (IEnumerable<string>) null;
          file = (string) null;
        }
        strArray = (string[]) null;
        this.ScheduleBatches();
        await Task.WhenAll((IEnumerable<Task>) this.processTasks);
        this.Logger.Error((object) "All process completed");
      }
      catch (Exception ex)
      {
        this.Logger.Error((object) ("Error Process - " + ex.Message));
      }
    }

    private void ScheduleBatches()
    {
      foreach (string file in Directory.GetFiles(this.folderName + "\\batches\\"))
        this.processTasks.Add(this.StartProcessAsync(this.Library.Environment.GetFolderList("smp$programs").Base.FullName + "\\SampleManagerCommand.exe", "-instance " + this.Library.Environment.InstanceName + " -username SERVICE -password  -task RunDataFix -FileName " + file));
    }

    private async Task StartProcessAsync(string processName, string arguments)
    {
      this.Logger.Error((object) ("Starting Process - " + arguments));
      ProcessStartInfo startInfo = new ProcessStartInfo()
      {
        FileName = processName,
        Arguments = arguments,
        UseShellExecute = true,
        WindowStyle = ProcessWindowStyle.Hidden
      };
      using (Process process = Process.Start(startInfo))
      {
        if (process != null)
        {
          await process.WaitForExitAsyncExt();
          this.Logger.Error((object) ("Process Completed - " + arguments));
        }
        else
          this.Logger.Error((object) ("Failed to Start the Process - " + arguments));
      }
      startInfo = (ProcessStartInfo) null;
    }
  }
}

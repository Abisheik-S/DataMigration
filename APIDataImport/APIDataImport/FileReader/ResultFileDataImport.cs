// Decompiled with JetBrains decompiler
// Type: APIDataImport.FileReader.ResultFileDataImport
// Assembly: APIDataImport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F18B3FE-91D4-415D-BCFE-CA5883100F9A
// Assembly location: C:\Users\Abisheik.S\Documents\Hudbay\APIDataImport.dll

using APIDataImport.Base;
using APIDataImport.DataImport;
using APIDataImport.Interfaces;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Thermo.SampleManager.Common.Data;
using Thermo.SampleManager.Library.EntityDefinition;
using Thermo.SampleManager.ObjectModel;

namespace APIDataImport.FileReader
{
  public class ResultFileDataImport : FileDataImport
  {
    public ResultFileDataImport(IDataImport<IEntity> ImportEntity)
      : base(ImportEntity)
    {
    }

    public override void ProcessFile(params object[] properties)
    {
      string property1 = properties[0] as string;
      object property2 = properties[1];
      Sample sample = (Sample) null;
      ResultDataImport importEntity = this.ImportEntity as ResultDataImport;
      foreach (string readLine in File.ReadLines(property1))
      {
        if (!string.IsNullOrEmpty(readLine))
        {
          WEBResultados webResultados = JsonConvert.DeserializeObject<WEBResultados>(readLine);
          try
          {
            if (!string.IsNullOrEmpty(webResultados.ID_MUESTRA.ToString()))
            {
              sample = importEntity.CheckSampleExists(webResultados.ID_MUESTRA.ToString());
              if (sample == null)
              {
                importEntity.Log(string.Format("Resultado de muestra Ref. {0} no Ingresado. Muestra no encontrada", (object) webResultados.ID_MUESTRA));
                continue;
              }
            }
            // ISSUE: reference to a compiler-generated field
            if (ResultFileDataImport.\u003C\u003Eo__1.\u003C\u003Ep__0 == null)
            {
              // ISSUE: reference to a compiler-generated field
              ResultFileDataImport.\u003C\u003Eo__1.\u003C\u003Ep__0 = CallSite<\u003C\u003EA\u007B00000040\u007D<CallSite, object, string, LabAliasEntry>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "TryGetValue", (IEnumerable<Type>) null, typeof (ResultFileDataImport), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsOut, (string) null)
              }));
            }
            LabAliasEntry labAliasEntry;
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            ResultFileDataImport.\u003C\u003Eo__1.\u003C\u003Ep__0.Target((CallSite) ResultFileDataImport.\u003C\u003Eo__1.\u003C\u003Ep__0, property2, webResultados.ANALITO.Trim() + "-" + webResultados.UNIDAD.Trim(), ref labAliasEntry);
            if (labAliasEntry == null)
              importEntity.Log("Par Alias " + webResultados.ANALITO.Trim() + "-" + webResultados.UNIDAD.Trim() + " no Encontrado.");
            else
              importEntity.TransferResult((object) sample, (object) webResultados, (object) labAliasEntry);
          }
          catch (Exception ex)
          {
            importEntity.Log(string.Format("Error: Análisis {0} para ID Muestra {1} no contiene Alias asociado.|'{2}';'{3}';'{4}';'{5}';'{6}';'{7}';'{8}'| ErrMsg: {9} ", (object) webResultados.ANALITO, (object) webResultados.ID_MUESTRA, (object) webResultados.ID_METODO, (object) webResultados.ANALITO, (object) webResultados.UNIDAD, (object) webResultados.LIMITE_DETECCION, (object) webResultados.LIMITE_CUANTIFICACION, (object) ((SampleBase) sample).Product, (object) property1, (object) (ex.Message + ex.StackTrace)));
          }
        }
      }
      importEntity.Commit();
      importEntity.ProcessNewResult();
    }
  }
}

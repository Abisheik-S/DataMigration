// Decompiled with JetBrains decompiler
// Type: APIDataImport.FileReader.SampleFileDataImport
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
using Thermo.SampleManager.ObjectModel;

namespace APIDataImport.FileReader
{
  public class SampleFileDataImport : FileDataImport
  {
    public List<Sample> SampleLocalList = new List<Sample>();

    public SampleFileDataImport(IDataImport<IEntity> ImportEntity)
      : base(ImportEntity)
    {
    }

    public override void ProcessFile(params object[] properties)
    {
      object property1 = properties[0];
      LabAlias property2 = properties[1] as LabAlias;
      object property3 = properties[2];
      object property4 = properties[3];
      SampleDataImport importEntity = this.ImportEntity as SampleDataImport;
      if (!importEntity.IsComponentsValid(property2))
        return;
      Sample mSample = (Sample) null;
      foreach (string readLine in File.ReadLines((string) property1))
      {
        if (!string.IsNullOrEmpty(readLine))
        {
          WEBMuestras muestra = JsonConvert.DeserializeObject<WEBMuestras>(readLine);
          try
          {
            // ISSUE: reference to a compiler-generated field
            if (SampleFileDataImport.\u003C\u003Eo__2.\u003C\u003Ep__0 == null)
            {
              // ISSUE: reference to a compiler-generated field
              SampleFileDataImport.\u003C\u003Eo__2.\u003C\u003Ep__0 = CallSite<\u003C\u003EA\u007B00000040\u007D<CallSite, object, string, LabAliasEntry>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "TryGetValue", (IEnumerable<Type>) null, typeof (SampleFileDataImport), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsOut, (string) null)
              }));
            }
            LabAliasEntry samplelabAliasEntry;
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            SampleFileDataImport.\u003C\u003Eo__2.\u003C\u003Ep__0.Target((CallSite) SampleFileDataImport.\u003C\u003Eo__2.\u003C\u003Ep__0, property3, muestra.TIPO_MUESTRA, ref samplelabAliasEntry);
            // ISSUE: reference to a compiler-generated field
            if (SampleFileDataImport.\u003C\u003Eo__2.\u003C\u003Ep__1 == null)
            {
              // ISSUE: reference to a compiler-generated field
              SampleFileDataImport.\u003C\u003Eo__2.\u003C\u003Ep__1 = CallSite<\u003C\u003EA\u007B00000040\u007D<CallSite, object, string, LabAliasEntry>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "TryGetValue", (IEnumerable<Type>) null, typeof (SampleFileDataImport), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsOut, (string) null)
              }));
            }
            LabAliasEntry labAliasEntry;
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            SampleFileDataImport.\u003C\u003Eo__2.\u003C\u003Ep__1.Target((CallSite) SampleFileDataImport.\u003C\u003Eo__2.\u003C\u003Ep__1, property4, muestra.TIPO_MUESTRA, ref labAliasEntry);
            if (samplelabAliasEntry != null)
            {
              if (importEntity.IsSampleNew(samplelabAliasEntry, muestra, ref mSample))
              {
                IEntity ientity = importEntity.Create(new object[3]
                {
                  (object) samplelabAliasEntry,
                  (object) muestra,
                  (object) labAliasEntry
                });
                if (mSample != null)
                  this.SampleLocalList.Add(ientity as Sample);
              }
              else
                importEntity.Update(new object[4]
                {
                  (object) mSample,
                  (object) samplelabAliasEntry,
                  (object) labAliasEntry,
                  (object) muestra
                });
            }
          }
          catch (Exception ex)
          {
            importEntity.Log(string.Format("Archivo: {0} | Error: Matriz {1} para Muestra ID {2} no contiene Alias asociado. Debe Crear el Alias {3} en el Alias de Matrices o revisar si existe el punto de Muestreo {4} | {5}", property1, (object) muestra.TIPO_MUESTRA, (object) muestra.ID_MUESTRA, (object) muestra.TIPO_MUESTRA, (object) muestra.NOMBRE_ESTACION, (object) (ex.Message + ex.StackTrace)));
          }
        }
      }
      importEntity.Commit();
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: APIDataImport.Base.FileDataImport
// Assembly: APIDataImport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F18B3FE-91D4-415D-BCFE-CA5883100F9A
// Assembly location: C:\Users\Abisheik.S\Documents\Hudbay\APIDataImport.dll

using APIDataImport.Interfaces;
using Thermo.SampleManager.Common.Data;

namespace APIDataImport.Base
{
  public abstract class FileDataImport
  {
    protected IDataImport<IEntity> ImportEntity;

    public FileDataImport(IDataImport<IEntity> ImportEntity) => this.ImportEntity = ImportEntity;

    public abstract void ProcessFile(params object[] properties);
  }
}

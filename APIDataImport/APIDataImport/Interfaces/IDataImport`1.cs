

namespace APIDataImport.Interfaces
{
  public interface IDataImport<AnyType>
  {
    AnyType Create(params object[] properties);

    AnyType Update(params object[] properties);

    void Commit();
  }
}

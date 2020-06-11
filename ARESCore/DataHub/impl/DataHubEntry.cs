using System;

namespace ARESCore.DataHub.impl
{
  public class DataHubEntry : IDataHubEntry
  {
    public Type DataType { get; set; }
    public object Data { get; set; }
    public string Source { get; set; }

    public DataHubEntry()
    {

    }

    public DataHubEntry(Type dataType, object data)
    {
      DataType = dataType;
      Data = data;
    }

    public DataHubEntry(Type dataType, object data, string dataSource)
    {
      DataType = dataType;
      Data = data;
      Source = dataSource;
    }
  }
}
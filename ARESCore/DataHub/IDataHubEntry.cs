using System;

namespace ARESCore.DataHub
{
  public interface IDataHubEntry
  {
    Type DataType { get; set; }
    Object Data { get; set; }
    String Source { get; set; }
  }
}

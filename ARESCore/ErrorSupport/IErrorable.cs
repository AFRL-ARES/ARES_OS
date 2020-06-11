using System.Collections.Generic;
using System.Threading.Tasks;
using ARESCore.ErrorSupport.Impl;

namespace ARESCore.ErrorSupport
{
  public interface IErrorable
  {
    List<ErrorResponse> AvailableErrorResponses { get; }

    IAresError Error { get; set; }

    Task Handle( ErrorResponse response );
  }
}

using DynamicData.Binding;

namespace ARESCore.ErrorSupport.Impl
{
  public class ErroredBundle: IErroredBundle
  {
    public IErrorable ErrorHandler { get; set; }
    public IErrorable ErrorNotifier { get; set; }
  }
}

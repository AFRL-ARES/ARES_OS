using System.Collections.ObjectModel;
using ARESCore.PluginSupport;
using Prism.Ioc;

namespace ARESCore.DeviceSupport
{
  public interface IAresDevice: IAresPlugin
  {
    void Init();
    void Activate();
    string Name { get; set; }
    IAresDeviceState CurrentState { get; set; }
    IAresDeviceState TargetState { get; set; }
    void IssueCommand( IAresDeviceCommand command);
    void RegisterCommands( IContainerRegistry registry );
    string GetSampleScriptEntry();
    int ReadRate { get; set; }
    string Validate();
    bool Connected { get; }
  }
}

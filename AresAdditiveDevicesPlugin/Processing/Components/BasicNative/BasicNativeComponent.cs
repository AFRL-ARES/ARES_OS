using AresAdditiveDevicesPlugin.Processing.Components.Base;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.Processing.Components.BasicNative
{
    public abstract class BasicNativeComponent : BasicComponent
    {
        public override string ComponentName
        {
            get => $"{_name}";
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public static string HumanReadableTypeString = "Native";

        public override string ToString()
        {
            return ComponentName;
        }
    }
}

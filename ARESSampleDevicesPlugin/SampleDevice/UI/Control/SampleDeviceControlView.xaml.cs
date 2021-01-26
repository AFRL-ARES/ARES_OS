using System.Windows.Input;

namespace AresSampleDevicesPlugin.SampleDevice.UI.Control
{

  public partial class SampleDeviceControlView
  {
    public SampleDeviceControlView()
    {
      InitializeComponent();
    }

    private void NumericUpDownKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        (DataContext as SampleDeviceControlViewModel).CommitDoubleValue();
      }
    }
  }
}

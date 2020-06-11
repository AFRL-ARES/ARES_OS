using AresAdditiveDevicesPlugin.Processing.Impl;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AresAdditiveDevicesPlugin.Processing.Components.BasicNative
{
  public class ConvertToGrayscale : BasicNativeComponent
  {
    public ConvertToGrayscale()
    {
      ComponentName = "ConvertToGrayscale";
      DefaultInputs.Add(new ProcessData<IInputArray>("Input", new Mat()));
      DefaultInputs.Add(new ProcessData<IInputArray>("Output", new Mat()));
    }
    public override Task StartComponent(IList<IProcessData> inputs)
    {
      var firstIn = inputs.First(input => input.Name.ToLower().Equals("Input".ToLower()));
      var output = inputs.First(input => input.Name.ToLower().Equals("Output".ToLower()));

      output.Data = ((Mat)firstIn.Data).ToImage<Gray, byte>();

      return Task.CompletedTask;
    }

  }
}

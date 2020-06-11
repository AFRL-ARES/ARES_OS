using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.Processing.Components.UserDefined;

namespace AresAdditiveDevicesPlugin.Experiment
{
  public interface IExperiment
  {
    Task<bool> Run(IExperimentParameters parameters, BasicUserDefinedComponent pipeline);
    IExperimentParameters Parameters { get; set; }
    double Results { get; set; }
    int Number { get; set; }
  }
}

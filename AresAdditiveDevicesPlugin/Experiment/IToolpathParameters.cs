using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.Experiment.Impl;

namespace AresAdditiveDevicesPlugin.Experiment
{
  public interface IToolpathParameters : IDictionary<string, VarEntry>
  {
  }
}

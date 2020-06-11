using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ARESCore.DeviceSupport;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace ARESCore.Experiment.Scripting.Impl
{
    public class ScriptStep : BasicReactiveObjectDisposable
    {
      private ObservableCollection<IAresDeviceCommand> _stepCommands = new ObservableCollection<IAresDeviceCommand>();
      private string _stepName;
      private bool _sequentialExecution;
      private string _stepText;

      public Guid StepId { get; set; }

      public string StepName
      {
        get => _stepName;
        set => this.RaiseAndSetIfChanged( ref _stepName, value);
      }

      public bool SequentialExecution
      {
        get => _sequentialExecution;
        set => this.RaiseAndSetIfChanged(ref _sequentialExecution, value);
      }

      public string StepText
      {
        get => _stepText;
        set => this.RaiseAndSetIfChanged(ref _stepText, value);
      }
      public ObservableCollection<IAresDeviceCommand> StepCommands
        {
            get => _stepCommands;
        set => this.RaiseAndSetIfChanged(ref _stepCommands, value);
      }

        public ScriptStep()
        {
            StepId = Guid.NewGuid();
            StepName = "";
        }
    }
}

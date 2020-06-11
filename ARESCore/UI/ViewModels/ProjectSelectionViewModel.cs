using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ARESCore.Configurations;
using ARESCore.DisposePatternHelpers;
using ARESCore.UserSession;
using Ninject;
using Prism.Interactivity.InteractionRequest;
using ReactiveUI;

namespace ARESCore.UI.ViewModels
{
  public class ProjectSelectionViewModel : BasicReactiveObjectDisposable
  {
    private bool _newProjectDefinitionComplete;
    private string _newProjectWorkingDirectory = "Click here to select project directory";
    private string _newProjectName = "";
    private IApplicationConfiguration _config;
    private IProjectInfo _selectedProject;

    public ProjectSelectionViewModel()
    {
      NotificationRequest = new InteractionRequest<INotification>();
      CreateNewProjectCommand = ReactiveCommand.Create<Unit>( u => CreateProject() );
      LoadProjectCommand = ReactiveCommand.Create<Unit>( u => LoadProject() );
      Config = AresKernel._kernel.Get<IApplicationConfiguration>();
      var currConfig = AresKernel._kernel.Get<ICurrentConfig>();
      if (currConfig.Project != null)
      {
        SelectedProject = currConfig.Project;
      }
    }

    private void LoadProject()
    {
      var currConfig = AresKernel._kernel.Get<ICurrentConfig>();
      currConfig.Project = SelectedProject;
      SelectedProject.LastLoadedDate = DateTime.Now;
      Config.SaveConfig( Config.CurrentAppConfigPath );
      var notif = new AresNotification
      {
        Title = "Notice",
        Content = "Project " + SelectedProject.Description + " Selected."
      };
      NotificationRequest.Raise( notif );
    }

    private void CreateProject()
    {
      var console = AresKernel._kernel.Get<IAresConsole>();
      console.WriteLine( "=====CREATING NEW PROJECT=======" );
      console.WriteLine( "Name : " + NewProjectName );
      console.WriteLine( "Working Directory: " + NewProjectWorkingDirectory );
      var newproj = NewProjectName;
      var newprojDir = NewProjectWorkingDirectory;
      console.WriteLine( "================================" );
      var proj = AresKernel._kernel.Get<IProjectInfo>();

      var currConfig = AresKernel._kernel.Get<ICurrentConfig>();
      proj.Creator = currConfig.User.Username;
      proj.Description = newproj;
      proj.LastLoadedDate = DateTime.Now;
      proj.SaveDirectory = newprojDir;
      proj.CreateDirectories();
      currConfig.Project = proj;
      Config.ProjectList.Add( proj );
      Config.SaveConfig( Config.CurrentAppConfigPath );
      NewProjectWorkingDirectory = "Click here to select project directory";
      NewProjectName = "";
      var notif = new AresNotification
      {
        Title = "Notice",
        Content = "Project " + newproj + " Created and Selected."
      };
      NotificationRequest.Raise( notif );
      return;
    }

    public InteractionRequest<INotification> NotificationRequest { get; set; }

    public bool NewProjectDefinitionComplete
    {
      get => _newProjectDefinitionComplete;
      set => this.RaiseAndSetIfChanged( ref _newProjectDefinitionComplete, value );
    }

    public string NewProjectWorkingDirectory
    {
      get => _newProjectWorkingDirectory;
      set
      {
        this.RaiseAndSetIfChanged( ref _newProjectWorkingDirectory, value );
        CheckNewProjComplete();
      }
    }

    public ReactiveCommand<Unit, Unit> CreateNewProjectCommand { get; set; }

    public ReactiveCommand<Unit, Unit> LoadProjectCommand { get; set; }

    public string NewProjectName
    {
      get => _newProjectName;
      set
      {
        this.RaiseAndSetIfChanged( ref _newProjectName, value );
        CheckNewProjComplete();
      }
    }

    public IApplicationConfiguration Config
    {
      get { return _config; }
      set { this.RaiseAndSetIfChanged( ref _config, value ); }
    }

    public IProjectInfo SelectedProject
    {
      get { return _selectedProject; }
      set { this.RaiseAndSetIfChanged( ref _selectedProject, value ); }
    }

    private void CheckNewProjComplete()
    {
      if ( NewProjectName.Length > 0 && NewProjectWorkingDirectory.Length > 0 && NewProjectWorkingDirectory.Contains( "\\" ) && Directory.Exists( NewProjectWorkingDirectory ) )
      {
        NewProjectDefinitionComplete = true;
      }
      else
      {
        NewProjectDefinitionComplete = false;
      }
    }
  }
}
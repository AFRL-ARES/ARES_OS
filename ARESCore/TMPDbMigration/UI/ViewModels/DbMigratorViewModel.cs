﻿using System.Reactive;
using ARESCore.DisposePatternHelpers;
using ARESCore.TMPDbMigration.Migrators;
using Ninject;
using ReactiveUI;

namespace ARESCore.TMPDbMigration.UI.ViewModels
{
  public class DbMigratorViewModel : BasicReactiveObjectDisposable
  {
    private ICollectionMigrator _migrator;

    public DbMigratorViewModel()
    {
      Test = ReactiveCommand.Create<Unit>(_ => MigrateStuff());
    }

    private async void MigrateStuff()
    {
      _migrator = AresKernel._kernel.Get<ICollectionMigrator>();
      var msPath = $"D:\\ARES\\Misc\\Reference DB Entries\\machinestates.json";
      await _migrator.MigrateFile(msPath, false);
      var expPath = $"D:\\ARES\\Misc\\Reference DB Entries\\experiments.json";
      await _migrator.MigrateFile(expPath, false);
      var dataPath = $"D:\\ARES\\Misc\\Reference DB Entries\\data.json";
      await _migrator.MigrateFile(dataPath, false);
      var planningPath = $"D:\\ARES\\Misc\\Reference DB Entries\\planning.json";
      await _migrator.MigrateFile(planningPath, false);
    }

    public ReactiveCommand<Unit, Unit> Test { get; }

    public ICollectionMigrator Migrator
    {
      get => _migrator;
      set => this.RaiseAndSetIfChanged(ref _migrator, value);
    }
  }
}
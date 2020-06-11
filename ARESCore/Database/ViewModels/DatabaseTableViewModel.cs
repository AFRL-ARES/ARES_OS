using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ARESCore.Database.Filtering;
using ARESCore.Database.Management.Impl;
using ARESCore.Database.Tables;
using ARESCore.DisposePatternHelpers;
using ARESCore.Extensions;
using Ninject;
using ReactiveUI;

namespace ARESCore.Database.ViewModels
{
  public class DatabaseTableViewModel : BasicReactiveObjectDisposable
  {
    private readonly IDbFilterManager _filterManager;
    private readonly IDbColumnCreator _columnCreator;
    private bool _databaseVisible;

    public DatabaseTableViewModel( IDbFilterManager filterManager, IDbColumnCreator columnCreator )
    {
      _filterManager = filterManager;
      _columnCreator = columnCreator;
      RowEntries = new ObservableCollection<ObservableCollection<string>>();
      _filterManager.SubscribeAndInvoke( fm => fm.LastFilterResult, fm => Update( fm.LastFilterResult ) );
    }

    private void Update( IEnumerable dataList )
    {
      if ( dataList == null )
        return;
      var filteredData = dataList as IEnumerable<ExperimentEntity>;

      var tempDbRows = new List<List<string>>();
      bool builtRows = false;
      foreach ( var entry in filteredData )
      {
        if ( !builtRows )
        {
          var ppeSet = AresKernel._kernel.Get<AresContext>().SetOf<IPostProcessEntity>();
          var ppe = ppeSet.FirstOrDefault( p => p.Id.Equals( entry.PostProcessData ) );
          tempDbRows.AddRange( _columnCreator.GetRows( ppe.GetType() ) );
          builtRows = true;
        }
      }
      DatabaseColumns.Clear();
      if(tempDbRows.Count == 0)
      {
        DatabaseVisible = false;
        return;
      }
      DatabaseColumns.AddRange( tempDbRows[0]);
      Application.Current.Dispatcher.BeginInvoke(new Action( () => 
      {
        RowEntries.Clear();
        for ( var index = 1; index < tempDbRows.Count; index++ )
        {
          var row = tempDbRows[index];
          var obrow = new ObservableCollection<string>();
          obrow.AddRange( row );
          RowEntries.Add( obrow );
        }
      }));
      DatabaseVisible = true;
    }

    public ObservableCollection<ObservableCollection<string>> RowEntries { get; set; }

    public ObservableCollection<string> DatabaseColumns { get; set; } = new ObservableCollection<string>();


    public bool DatabaseVisible
    {
      get => _databaseVisible;
      set => this.RaiseAndSetIfChanged( ref _databaseVisible, value );
    }
  }
}
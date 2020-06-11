using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARESCore.Database.Management.Impl;
using ARESCore.Database.Tables;
using ARESCore.DisposePatternHelpers;
using ARESCore.UI;
using CommonServiceLocator;
using Ninject;

namespace ARESCore.Database.Filtering.Impl
{
  public class MachineStateFilter : BasicReactiveObjectDisposable, IDbFilter<MachineStateEntity>
  {
    private readonly IAresConsole _console;

    public MachineStateFilter( IAresConsole console )
    {
      _console = console;
    }

    public IEnumerable<MachineStateEntity> Filter( IEnumerable<MachineStateEntity> mDocs )
    {
      return mDocs;
      //      GetCount( mDocs, "Prefiltered machine state entries" );
      //      var opts = AresKernel._kernel.Get<IMachineStateFilterOptions>();
      //      var query =
      //        from mDoc in mDocs
      // TODO FIXME Build query via opts
      //        select mDoc;

      //      GetCount( query, "Postfiltered machine state entries" );
      //      return query;
    }

    public IEnumerable<ExperimentEntity> FilterExperimentsOn( IEnumerable<ExperimentEntity> expData, IEnumerable<MachineStateEntity> typeData )
    {
      var opts = ServiceLocator.Current.GetInstance<IMachineStateFilterOptions>();
      if ( !opts.FilterChipDescriptions )
      {
        return expData;
      }
      // TODO FIXME
      return expData;
    }


    public void GetCount( IEnumerable<MachineStateEntity> docs, string desc = "" )
    {
      _console.WriteLine( desc + ": " + docs.Count() );
    }
  }
}
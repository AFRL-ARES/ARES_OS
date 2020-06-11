using System;
using System.Collections.Generic;
using System.Linq;
using ARESCore.Database.Management.Impl;
using ARESCore.Database.Tables;
using ARESCore.UI;
using Ninject;

namespace ARESCore.Database.Filtering.Impl
{
  public class ExperimentFilter : IExperimentFilter
  {
    private readonly IAresConsole _console;

    public ExperimentFilter( IAresConsole console )
    {
      _console = console;
    }
    public IEnumerable<ExperimentEntity> Filter( IEnumerable<ExperimentEntity> preFilteredData )
    {
      var context = AresKernel._kernel.Get<AresContext>();
      var exprs = context.Experiments;
      var opts = AresKernel._kernel.Get<IExperimentFilterOptions>();
      var exps = exprs.AsEnumerable().ToList();
      GetCount( exps, "Prefiltered Experiments" );
     

      var subset = opts.FilterProjectDescription ? exps.Where( e => opts.Projects.Contains( e.Project ) ) : exps;
      subset = opts.FilterExperimentDate ? subset.Where( exp => exp.TimeStamp >= opts.FromDate && exp.TimeStamp < opts.ToDate ) : subset;
      if ( opts.FilterBatchType )
      {
        var optType = opts.BatchTypeFilter.DbTypeSupported;
        var ppds = context.SetOf<IPostProcessEntity>();
        if ( ppds == null )
        {
          subset = new List<ExperimentEntity>();
        }
        
        subset = subset.Where( exp => ppds.FirstOrDefault(
          e => 
          e.Id.Equals( exp.PostProcessData ) )?.GetType() == optType
        ).ToList();
      }
//      .Where !( opts.FilterBatchType && opts.BatchTypeFilter == ARESExperimentBatch.BatchType.CNT_GROWTH && opts.FilterCNTR2Minimum ) ||
      //      (
      //        // This first check makes sure that the PostProcessData is of type CNTGrowthPostProcessing
      //        // It does not throw and exception when it is of another type, it simply is false 
      //        // (I assume it "casts" as a Json/Bson document and looks for a section titled "GBandData", if it exists, it continues its check)
      //        ( (CNTGrowthPostProcessing)eDoc.PostProcessData ).GBandData != null &&
      //        ( (CNTGrowthPostProcessing)eDoc.PostProcessData ).GBandData.RSquare > opts.R2Minimum
      //      )
//      .Where !( opts.FilterBatchType && opts.BatchTypeFilter == ARESExperimentBatch.BatchType.CNT_GROWTH ) ||
      //      (
      //        // Uses same trick as above to ensure the PostProcessData is of type CNTGrowthPostProcessing
      //        ( (CNTGrowthPostProcessing)eDoc.PostProcessData ).GBandData != null
      //      )
//      .Where !( opts.FilterBatchType && opts.BatchTypeFilter == ARESExperimentBatch.BatchType.GRAPHENE_DEFECT ) ||
      //      (
      //        // Uses same trick as above to ensure the PostProcessData is of type GrapheneDefectPostProcessing
      //        ( (GrapheneDefectPostProcessing)eDoc.PostProcessData ).GrapheneData != null
      //      )
        GetCount( subset, "Postfiltered Experiments" );
        
      return subset.ToList();
    }


    public List<ExperimentEntity> FilterExperiments( FilterOptions opts )
    {
      throw new NotImplementedException();
    }

    public IQueryable<ExperimentEntity> FilterExperimentsOnPlans( FilterOptions opts, IQueryable<ExperimentEntity> initSet )
    {
      GetCount( initSet, "Filtering on Plans" );
      if ( !opts.FilterPlannerType )
        return initSet;

      var exps =
        from exp in initSet
        where !exp.Planner.Equals( Guid.Empty )
        select exp.Planner;
      var eDocPDocIdsList = exps.ToList();
      var context = AresKernel._kernel.Get<AresContext>();
      var planners =
        from planner in context.Planners
        where eDocPDocIdsList.Contains( planner.Id )
        // .Where !opts.FilterPlannerType || pDoc.PlannerType == opts.PlannerDocFilterType
        select planner.Id;
      var matchingPDocIdsList = planners.ToList();

      var query =
        from eDoc in initSet
        where matchingPDocIdsList.Contains( eDoc.Planner )
        select eDoc;

      GetCount( query, "Postfiltered Plans" );
      return query;
    }

    public IQueryable<ExperimentEntity> FilterExperimentsOnMachine( FilterOptions opts, IQueryable<ExperimentEntity> initSet )
    {
      GetCount( initSet, "Filtering on machine state" );
      if ( !opts.FilterChipDescriptions )
        return initSet;

      var eDocMDocIds =
        from experiment in initSet
        where experiment.MachineStates.Length > 0
        select experiment.MachineStates[0];
      var machineStateIdsFromExperiments = eDocMDocIds.ToList();

      var context = AresKernel._kernel.Get<AresContext>();
      var matchingMachineStates =
        from machineState in context.MachineStates
        where machineStateIdsFromExperiments.Contains( machineState.Id )
        //          .Where !opts.FilterChipDescriptions || opts.Chips.Contains( mDoc.ChannelStates.ChipState.ChipID ) // TODO FIXME
        select machineState.Id;
      var matchingMachineStateList = matchingMachineStates.ToList();

      var query =
        from experiment in initSet
        where experiment.MachineStates.Length > 0
        where matchingMachineStateList.Any( mDocId => experiment.MachineStates.Contains( mDocId ) )
        select experiment;

      GetCount( query, "MDoc.End" );
      return query;
    }

    public IQueryable<ExperimentEntity> FilterExperimentsOnData( FilterOptions opts, IQueryable<ExperimentEntity> initSet )
    {
      GetCount( initSet, "Filtering on Data" );
      //if ( !( opts.FilterBatchType && opts.BatchTypeFilter == ARESExperimentBatch.BatchType.CNT_GROWTH && opts.FilterCNTGBandAvg ) )
      //   return eDocs;

      var eDocRDocIds =
        from eDoc in initSet
        where eDoc.Data.Count() > 0
        select new { EDocId = eDoc.Id, RDocIds = eDoc.Data };
      var eDocRDocIdsList = eDocRDocIds.ToList();

      /*var eDocRDocIdsFinalList = eDocRDocIdsList.Select( erDoc =>
       {
         var rDocGBand =
                  from rDoc in RamanCollection.AsQueryable()
                  .Where erDoc.RDocIds.Contains( rDoc.SpectrumId )
                  .Where rDoc.AOIDataPoints.Count > 0 && rDoc.AOIDataPoints.Any( aoi => aoi.AOIInfo.Type == AreaOfInterest.AOIType.GDBAND )
                  //select rDoc.AOIDataPoints..Where(aoi => aoi.AOIInfo.Type == AreaOfInterest.AOIType.GDBAND).Cast<GDBandAOIData>().First().GBandArea;
                  select rDoc.AOIDataPoints..Where( aoi => aoi.AOIInfo.Type == AreaOfInterest.AOIType.GDBAND ).First();

         var rDocGBandAvg = 0.0;
         var rDocGBandAoiList = rDocGBand.ToList().Select( aoi => ( (GDBandAOIData)aoi ).GBandArea ).ToList();
         if ( rDocGBandAoiList.Count > 0 )
           rDocGBandAvg = rDocGBandAoiList.Average();

         return new { EDocId = erDoc.EDocId, GBandAvg = rDocGBandAvg };
       } )..Where( erDoc => erDoc.GBandAvg > opts.GBandAvgMinimum ).Select( erDoc => erDoc.EDocId ).ToList();*/

      var query =
        from eDoc in initSet
        //      .Where eDocRDocIdsFinalList.Contains( eDoc.ExperimentId )
        select eDoc;

      GetCount( query, "Postfiltered data" );
      return query;
    }

    public void GetCount( IEnumerable<ExperimentEntity> docs, string desc = "" )
    {
      _console.WriteLine( desc + ": " + docs.Count() );
    }

    public IEnumerable<ExperimentEntity> FilterExperimentsOn( IEnumerable<ExperimentEntity> expData, IEnumerable<ExperimentEntity> typeData )
    {
      return expData;
    }
  }
}
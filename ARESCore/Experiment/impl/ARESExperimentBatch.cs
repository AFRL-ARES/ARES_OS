using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ARESCore.AnalysisSupport;
using ARESCore.Database.Filtering;
using ARESCore.Database.Filtering.Impl;
using ARESCore.Database.Management.Impl;
using ARESCore.Database.Tables;
using ARESCore.DeviceSupport;
using ARESCore.Experiment.Scripting;
using ARESCore.Experiment.Scripting.Impl;
using ARESCore.PlanningSupport;
using ARESCore.UI;
using ARESCore.UserSession;
using Ninject;

namespace ARESCore.Experiment.impl
{
  public class ARESExperimentBatch : IExperimentBatch
  {
    private AresContext _context;

    public enum DataSaveType { CSV, JSON, ALL };

    public IPlannedExperimentBatchInputs Inputs { get { return BatchInputs; } set { } }
    public IAresAnalyzer Analyzer { get; set; }
    public IPlannedExperimentBatchInputs BatchInputs { get; set; } = new PlannedExperimentBatchInputs();
    public IAresPlannerManager PlannerManager { get; set; } = null;
    public ObservableCollection<ExperimentEntity> PlanningDatabase { get; set; } = new ObservableCollection<ExperimentEntity>();
    public ExperimentFilterOptions PlannerExperimentFilterOptions { get; set; } = null;
    public DataSaveType SaveType { get; set; } = DataSaveType.CSV;

    public string RawExperimentScript { get; set; } = "";
    public ObservableCollection<ScriptStep> RecipeSteps { get; set; } = new ObservableCollection<ScriptStep>();

    public bool DoReplan { get; set; } = false;
    public int ReplanAfter { get; set; } = 0;
    public int MaxExperiments { get; set; } = int.MaxValue;

    // Running Parameters
    public Guid PlannerDocID { get; protected set; }
    public ExperimentEntity CurrentExperimentDoc { get; protected set; }
    public List<ExperimentEntity> AllExperimentDocs { get; protected set; }
    public List<DataEntity> CurrentSpectrums { get; protected set; }
    public List<DataEntity> AllCollectedSpectrums { get; protected set; }
    public List<MachineStateEntity> CurrentMachineStates { get; protected set; }
    public List<MachineStateEntity> AllCollectedMachineStates { get; protected set; }

    public string PlannedExperimentBatchInputsPath { get; protected set; }

    public ARESExperimentBatch()
    {
      Inputs = null;
      _context = AresKernel._kernel.Get<AresContext>();
    }

    // [Step 1.a; called once before batch execution] Called to verify everything is in line and ready to run
    public virtual Task VerifyExperimentBatch( CancellationToken cancelToken )
    {
      // Ensure that the PlannedExperimentInputs is correct
      if ( !( Inputs is PlannedExperimentBatchInputs ) )
        throw new Exception( "Experiment batch inputs corrupt!" );

      // Ensure that an Experiment recepie exists
      if ( RecipeSteps == null )
        throw new Exception( "Experiment batch recipe corrupt!" );

      // Ensure that all hardware is working
      var allDevices = AresKernel._kernel.GetAll<IAresDevice>();
      foreach ( var aresDevice in allDevices )
      {
        string errorStr = aresDevice.Validate();
        if ( errorStr != null )
          throw new Exception( "Hardware error: " + errorStr );
      }

      // Ensure the current pillar is set
      // TODO FIXME
      //      if ( MainForm.Instance.ARESNanoLab.DataChannelState.ChipState.CurrentPillarIndex == null )
      //        throw new Exception( "Current pillar index is set to null! Please align a pillar before proceeding!" );

      // FIXME: Check the selected batch type and show stuff (like AOIs)
      // Just make completely sure the GDBand AOI is there, user can't delete it, but it could be removed from the RamanHardwareMainConfig possbily..
      //     if ( !AresKernel._kernel.Get<ICurrentConfig>().UserSession.HardwareMainConfig.RamanConfig.AreasOfInterest.Any( ( x ) => x.Type == AreaOfInterest.AOIType.GDBAND ) )
      //       AresKernel._kernel.Get<ICurrentConfig>().UserSession.HardwareMainConfig.RamanConfig.AreasOfInterest.Add( new GDBandAOI( "G-D Band", AreaOfInterest.AOIType.GDBAND, Color.FromArgb( 128, 255, 128 ), true, true ) );

      return Task.FromResult( (object)null );
    }

    // [Step 1.b; called once before batch execution] Called to startup and inputs collection needed
    public virtual async Task InitDataCollection( CancellationToken cancelToken )
    {
      CurrentExperimentDoc = new ExperimentEntity();
      AllExperimentDocs = new List<ExperimentEntity>();

      CurrentSpectrums = new List<DataEntity>();
      AllCollectedSpectrums = new List<DataEntity>();

      CurrentMachineStates = new List<MachineStateEntity>();
      AllCollectedMachineStates = new List<MachineStateEntity>();

      // Create a folder in which to save this experiment batch's inputs
      PlannedExperimentBatchInputsPath = AresKernel._kernel.Get<ICurrentConfig>().Project.SaveDirectory_ExportedBatchData; // Used to be UserSession.SaveDirectory
                                                                                                                  //PlannedExperimentBatchInputsPath += "\\ExperimentBatchResults\\ExperimentBatch_" +  DateTime.Now.ToString("ddd-MMM-d_HHmm");
                                                                                                                  // TODO FIXME
                                                                                                                  //      var chipState = MainForm.Instance.ARESNanoLab.DataChannelState.ChipState;
                                                                                                                  // TODO FIXME
                                                                                                                  //      string chipDescription = chipState.ChipDescription;
                                                                                                                  //      foreach ( char illegalChar in Path.GetInvalidFileNameChars() )
                                                                                                                  //        chipDescription = chipDescription.Replace( illegalChar.ToString(), "" );
                                                                                                                  //      PlannedExperimentBatchInputsPath += chipDescription + "\\" + "Patch_" + ( chipState.PatchIndex + 1 ) + "\\";
      Directory.CreateDirectory( PlannedExperimentBatchInputsPath );

      // Save the planner document if planning took place
      PlannerDocID = Guid.Empty;
      /*  if ( PlanningParameters != null )
        {
          var pDocToSave = new PlannerDatabaseEntry( (PlanningUtil.PlannerTypes)PlanningParameters.PlanningType, PlanningParameters, BatchData );

          PlannerDocID = pDocToSave.PlannerId;
          string plannerDocPath = PlannedExperimentBatchInputsPath + "\\PlannerDoc_" + PlannerDocID.ToString().Substring( 0, 8 ) + ".json";
          CurrentExperimentDoc.ChosenPlannerDocId = PlannerDocID;

          await PlannerDatabaseEntry.ExportAsJson( new List<PlannerDatabaseEntry>() { pDocToSave }, plannerDocPath, true );
          await AresKernel._kernel.Get<ARESMongoDBHistory>().PublishDocAsync( pDocToSave );
        }*/

      // Set the ExperimentEntitys project description
      CurrentExperimentDoc.Project = AresKernel._kernel.Get<ICurrentConfig>().Project.Description;
    }

    // [Step 2.a; called after each experiment] Called to do any inputs processing needed
    public virtual async Task DoCurrentExperimentPostProcessing( CancellationToken cancelToken )
    {
      //FIXME: Call the post processor. Ask the analyzer if we have enough experiments and to do the fit
      if ( CurrentSpectrums.Count < 5 ) // FIXME: Ask the analyzer about this.
        throw new Exception( "Not enough spectrums collected to fit GBand growth model to!" );
      try
      {
        //           CurrentExperimentDoc.PostProcessData = await CNTGrowthPostProcessing.CalculateFit( CurrentSpectrums, CurrentMachineStates );
      }
      catch ( Exception ex ) { throw new Exception( "CNTGrowthPostProcessing failed!", ex ); }
    }

    // [Step 2.b; called after each experiment] Called to save the inputs the experiment produced
    public virtual async Task SaveCurrentExperimentData( CancellationToken cancelToken )
    {
      // TODO FIXME
      // Create the path for this specific pillar in the save directory
      //      var chipState = MainForm.Instance.ARESNanoLab.DataChannelState.ChipState;

      //      string patchPillarLabel = "P" + ( chipState.PatchIndex + 1 ) + "p" + ( chipState.CurrentPillarIndex.X + 1 ) + ( chipState.CurrentPillarIndex.Y + 1 );
      //      string pillarLabel = "Pillar_" + ( chipState.CurrentPillarIndex.X + 1 ) + "-" + ( chipState.CurrentPillarIndex.Y + 1 );
      //      string ExperimentPath = PlannedExperimentBatchInputsPath + pillarLabel;

      //       Ensure that we do not mix inputs that was collected from the same pillar (consider this user error)
      //      int pillarExpIndex = 0;
      //      for ( ; Directory.Exists( ExperimentPath + "_Exp" + pillarExpIndex + "\\" ); pillarExpIndex++ )
      //        ;
      //      ExperimentPath += "_Exp" + pillarExpIndex + "\\";

      // Create the directory and save it to the eDoc
      //      Directory.CreateDirectory( ExperimentPath );
      //      CurrentExperimentDoc.OriginalWorkingFolder = ExperimentPath;

      // Serialize all documents to the actual database
//      await AresKernel._kernel.Get<ARESMongoDBConnection>().PublishDocAsync( CurrentExperimentDoc );
//      if ( CurrentMachineStates.Length > 0 )
//        await AresKernel._kernel.Get<ARESMongoDBConnection>().PublishDocsAsync( CurrentMachineStates );
//      if ( CurrentSpectrums.Count > 0 )
//        await AresKernel._kernel.Get<ARESMongoDBConnection>().PublishDocsAsync( CurrentSpectrums );

      //      string eDocPath = ExperimentPath;
      //      string mDocsPath = ExperimentPath + "MachineStates\\";
      //      string rDocsPath = ExperimentPath + "Raman\\";

      // Export as JSON files to the file system
//      if ( SaveType == DataSaveType.JSON || SaveType == DataSaveType.ALL )
//      {
//        //        if ( !await ExperimentEntity.ExportAsJson( new List<ExperimentEntity>() { CurrentExperimentDoc }, eDocPath + "ExperimentDoc.json", true ) )
//        AresKernel._kernel.Get<IAresConsole>().WriteLine( "# Failed to save ExperimentEntity.json for experiment: [" + CurrentExperimentDoc.ExperimentId + "] FromFolder: [" + CurrentExperimentDoc.OriginalWorkingFolder + "]" );
//
//        //        if ( CurrentMachineStates.Length > 0 && !await MachineState_DBDoc.ExportAsJson( CurrentMachineStates, mDocsPath ) )
//        AresKernel._kernel.Get<IAresConsole>().WriteLine( "# Failed to save MachineState_DBDoc[s].json for experiment: [" + CurrentExperimentDoc.ExperimentId + "] FromFolder: [" + CurrentExperimentDoc.OriginalWorkingFolder + "]" );
//
//        //     if ( CurrentSpectrums.Count > 0 && !await DataEntity.ExportAsJson( CurrentSpectrums, rDocsPath ) )
//        //      AresKernel._kernel.Get<IAresConsole>().WriteLine( "# Failed to save DataEntity[s].json for experiment: [" + CurrentExperimentDoc.ExperimentId + "] FromFolder: [" + CurrentExperimentDoc.OriginalWorkingFolder + "]" );
//      }
//
//      // Export CSV files to the file system
//      if ( SaveType == DataSaveType.CSV || SaveType == DataSaveType.ALL )
//      {
//        //        if ( !await ExperimentEntity.ExportExperimentReport( CurrentExperimentDoc, eDocPath + "ExperimentOverview_" + patchPillarLabel + ".txt" ) )
//        AresKernel._kernel.Get<IAresConsole>().WriteLine( "# Failed to save ExpOverviewFile for experiment: [" + CurrentExperimentDoc.ExperimentId + "] FromFolder: [" + CurrentExperimentDoc.OriginalWorkingFolder + "]" );
//
//        //        if ( CurrentMachineStates.Length > 0 && !await MachineState_DBDoc.ExportAsCSVReport( CurrentMachineStates, mDocsPath + "MachineStateOverview_" + patchPillarLabel + ".csv" ) )
//        AresKernel._kernel.Get<IAresConsole>().WriteLine( "# Failed to save MachStateOverview for experiment: [" + CurrentExperimentDoc.ExperimentId + "] FromFolder: [" + CurrentExperimentDoc.OriginalWorkingFolder + "]" );

        //      if ( CurrentSpectrums.Count > 0 && !await DataEntity.ExportAsCSVs( CurrentSpectrums, rDocsPath ) )
        //        AresKernel._kernel.Get<IAresConsole>().WriteLine( "# Failed to save RamanCalibratedFile for experiment: [" + CurrentExperimentDoc.ExperimentId + "] FromFolder: [" + CurrentExperimentDoc.OriginalWorkingFolder + "]" );
        //      if ( CurrentSpectrums.Count > 0 && !await DataEntity.ExportAOISummary( CurrentSpectrums, rDocsPath + "AOIData_" + patchPillarLabel + ".csv" ) )
        //        AresKernel._kernel.Get<IAresConsole>().WriteLine( "# Failed to save AOIDataFile for experiment: [" + CurrentExperimentDoc.ExperimentId + "] FromFolder: [" + CurrentExperimentDoc.OriginalWorkingFolder + "]" );
        //      if ( CurrentSpectrums.Count > 0 && !await DataEntity.ExportPashasFile( CurrentSpectrums, CurrentMachineStates, rDocsPath + "PashasFile_" + patchPillarLabel + ".csv" ) )
        //        AresKernel._kernel.Get<IAresConsole>().WriteLine( "# Failed to save PashasFile for experiment: [" + CurrentExperimentDoc.ExperimentId + "] FromFolder: [" + CurrentExperimentDoc.OriginalWorkingFolder + "]" );
//      }

      // Add experiment documents to planning database if it passes the filter
//      if ( PlannerFilterOptions != null )
//      {
//        /*if ( CurrentExperimentDoc.PostProcessData.GetType() == typeof( CNTGrowthPostProcessing ) && !double.IsNaN( PlannerFilterOptions.R2Minimum ) )
//        {
//          if ( ( (CNTGrowthPostProcessing)CurrentExperimentDoc.PostProcessData ).GBandData.RSquare > PlannerFilterOptions.R2Minimum )
//            PlanningDatabase.Add( CurrentExperimentDoc );
//          else
//          {
//            // If an experiment is present with a low R2 (bad fit), but low GBand area, then it was simply no growth. Report it to the planner with a very low Nu
//            var preScan = CurrentSpectrums.First();
//            var postScan = CurrentSpectrums.Last();
//            var growthScans = CurrentSpectrums.Where( rDoc => !rDoc.SpectrumId.Equals( preScan ) && !rDoc.SpectrumId.Equals( postScan ) ).ToList();
//            double avgGBandArea = growthScans
//                .Select( rDoc => (GDBandAOIData)rDoc.AOIDataPoints.Where( aoi => aoi.AOIInfo.Type == AreaOfInterest.AOIType.GDBAND ).First() )
//                .Select( gAoi => gAoi.GBandArea ).Average();
//
//            if ( avgGBandArea < PlannerFilterOptions.GBandAvgMinimum )
//            {
//              AresKernel._kernel.Get<IAresConsole>().WriteLine( "!! Experiment R2 falls below threshold, but average GBand area of " + avgGBandArea.ToString( "f0" ) + " is above threshold, adding to Planning Database !!" );
//              PlanningDatabase.Add( CurrentExperimentDoc );
//            }
//          }
//        }
//        else
//          PlanningDatabase.Add( CurrentExperimentDoc );*/
//      }
//      else
//        PlanningDatabase.Add( CurrentExperimentDoc );

      // Reinitialize these variables for the next experiment
      AllExperimentDocs.Add( CurrentExperimentDoc );
      CurrentExperimentDoc = new ExperimentEntity();
     // CurrentExperimentDoc.PlannerEntryId = PlannerDocID;
      CurrentExperimentDoc.Project = AresKernel._kernel.Get<ICurrentConfig>().Project.Description;

      AllCollectedSpectrums.AddRange( CurrentSpectrums );
      CurrentSpectrums = new List<DataEntity>();

      AllCollectedMachineStates.AddRange( CurrentMachineStates );
      CurrentMachineStates = new List<MachineStateEntity>();
    }

    // Caller needs to make sure that we only receive Spectrums and MachineStates that take place during the experiment
    // there is the issue that a Spectrum/MachineState could be collected while the ExperimentRunner is not running! (How do we fix this?)
    public virtual void NewNewtonCalibratedSpectrum( DataEntity newCalibDataDoc )
    {
      CurrentSpectrums.Add( newCalibDataDoc );
     // CurrentExperimentDoc.DataDBEntries.Add( newCalibDataDoc );
      // CurrentExperimentDoc.RamanDocIds.Add( newCalibDataDoc.SpectrumId );
    }
    public virtual void NewMachineState( MachineStateEntity newMachineStates )
    {
      CurrentMachineStates.Add( newMachineStates );
     var newList = CurrentExperimentDoc.MachineStates.ToList();
      newList.Add( newMachineStates.Id );
      _context.MachineStates.Add( newMachineStates );
    }



    // [Step 3; called after some # of experiments] Called to do replanning and replace the IPlannedExperimentInputs
    public virtual async Task DoReplanning( CancellationToken cancelToken )
    {
      //     if ( PlanningParameters == null || PlanningType == "NONE" )
      //        throw new Exception( "Replanning parameters or type are null!" );

      // Perform the actual planning
      Task<IPlannedExperimentBatchInputs> planningTask = null;
      /*     switch ( PlanningType )
           {
             case PlanningUtil.PlannerTypes.NMDM3:
               {
                 var NMDM3Params = PlanningParameters as NMDM3PlanningParameters;
                 if ( NMDM3Params != null )
                 {
                   NMDM3Params.PlanningDatabase = PlanningDatabase;
                   var planner = new NMDM3Planner( NMDM3Params );
                   planningTask = new NMDM3Planner( NMDM3Params ).DoPlanning( cancelToken, null );
                 }
                 else
                   throw new Exception( "Replanning type does not match replanning parameters type!" );
               }
               break;
             case PlanningUtil.PlannerTypes.HOLMES:
               {
                 var HOLMESParams = PlanningParameters as HOLMESPlanningParameters;
                 if ( HOLMESParams != null )
                 {
                   HOLMESParams.PlanningDatabase = PlanningDatabase;
                   planningTask = new HOLMESPlanner( HOLMESParams ).DoPlanning( cancelToken, null );
                 }
                 else
                   throw new Exception( "Replanning type does not match replanning parameters type!" );
               }
               break;
             case PlanningUtil.PlannerTypes.RAAS:
               {
                 var RAASParams = PlanningParameters as RAASPlanningParameters;
                 if ( RAASParams != null )
                 {
                   RAASParams.PlanningDatabase = PlanningDatabase;
                   planningTask = new RAASPlanner( RAASParams ).DoPlanning( cancelToken, null );
                 }
                 else
                   throw new Exception( "Replanning type does not match replanning parameters type!" );
               }
               break;
             case PlanningUtil.PlannerTypes.BORAAS:
               {
                 var BORAASParams = PlanningParameters as BORAASPlanningParameters;
                 if ( BORAASParams != null )
                 {
                   BORAASParams.PlanningDatabase = PlanningDatabase;
                   planningTask = new BORAASPlanner( BORAASParams ).DoPlanning( cancelToken, null );
                 }
                 else
                   throw new Exception( "Replanning type does not match replanning parameters type!" );
               }
               break;
             default:
             case "NONE":
               throw new Exception( "Invalid replanning type: " + PlanningType + "!" );
           }*/

      // Wait for planning to finish
      try
      { await planningTask; }
      catch ( Exception ex ) { throw new Exception( "Replanning failed!", ex ); }

      var newBatchData = planningTask.Result;

      // Save the planner document with the planning inputs it produced
      PlannerDocID = Guid.Empty;
      /*if ( PlanningParameters != null )
      {
        var pDocToSave = new PlannerDatabaseEntry( (PlanningUtil.PlannerTypes)PlanningParameters.PlanningType, PlanningParameters, newBatchData );
        PlannerDocID = pDocToSave.PlannerId;
        string plannerDocPath = PlannedExperimentBatchInputsPath + "\\PlannerDoc_" + PlannerDocID.ToString().Substring( 0, 8 ) + ".json";
        await PlannerDatabaseEntry.ExportAsJson( new List<PlannerDatabaseEntry>() { pDocToSave }, plannerDocPath, true );
        await AresKernel._kernel.Get<ARESMongoDBHistory>().PublishDocAsync( pDocToSave );
      }*/

      // Save the new batch inputs in the experiemnt batch, the runner will index it properly
      BatchInputs = newBatchData as PlannedExperimentBatchInputs;
    }

    // [Step 4; called once after batch execution] Called to perform an final post processing needed on the batch
    public virtual async Task DoBatchPostProcessing( CancellationToken cancelToken )
    {

    }
  }
}

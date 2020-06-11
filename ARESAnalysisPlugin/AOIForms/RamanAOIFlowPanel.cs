using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AresCNTAnalysisPlugin.Config;
using AresCNTAnalysisPlugin.Database;
using AresCNTAnalysisPlugin.RamanAnalysis.AreasOfInterest;
using AresCNTAnalysisPlugin.RamanAnalysis.PeakFitting;
using ARESCore.Database.Entries;
using ARESCore.Registries;
using CommonServiceLocator;
using DynamicData.Binding;

namespace AresCNTAnalysisPlugin.AOIForms
{
    public partial class RamanAOIFlowPanel : UserControl
    {
        private List<Control> AOIGraphs { get; set; } = new List<Control>();

        public RamanAOIFlowPanel()
        {
            InitializeComponent();
            ServiceLocator.Current.GetInstance<Database.DataDatabaseEntry>().WhenAnyPropertyChanged().Subscribe(t => NewData(t));
        }

        public void SetAreasOfInterest(List<AreaOfInterest> areasOfInterest)
        {
          //TODO: FixMe change this list type to IAoiGraph for the new graphs or change how this thing works all-together.
            AOIGraphs = new List<Control>();
            foreach (var aoi in areasOfInterest)
            {
                if (aoi.ShowAOIPanel)
                {
                    switch (aoi.Type)
                    {
                        case AreaOfInterest.AOIType.CUSTOM: AOIGraphs.Add(new CustomAOIGraph(aoi as CustomAOI)); break;
                        case AreaOfInterest.AOIType.GDBAND: AOIGraphs.Add(new GDBandAOIGraph(aoi as GDBandAOI)); break;
                        case AreaOfInterest.AOIType.SILICON:
                        {
                          var ramanConfig = ServiceLocator.Current.GetInstance<IConfigManagerRegistry>().FirstOrDefault( f => f.UserDeviceConfig is IRamanUserDeviceConfig ) as IRamanUserDeviceConfig;
                switch (ramanConfig.SiliconFitType)
                            {
                                case SiliconFitting.FitType.SaS_CHF:
                                case SiliconFitting.FitType.SaS_CHCF:
                                    AOIGraphs.Add(new SiliconSaSAOIGraph(aoi as SiliconAOI)); break;
                                default:
                                case SiliconFitting.FitType.S_CHCF:
                                    AOIGraphs.Add(new SiliconAOIGraph(aoi as SiliconAOI)); break;
                            }
                        }
                        break;
                    }
                }
            }
            SyncAOIBandsToPanel();
        }

        private void SyncAOIBandsToPanel()
        {
            flpGraphView.Controls.Clear();
            foreach (var graph in AOIGraphs) flpGraphView.Controls.Add(graph);
            SizeGraphs();
        }

        public void NewData(IDataDatabaseEntry newCalibDataDoc)
        {
            foreach(var graph in AOIGraphs)
            {
                if (graph is CustomAOIGraph) (graph as CustomAOIGraph).NewData(newCalibDataDoc);
                else if (graph is GDBandAOIGraph) (graph as GDBandAOIGraph).NewData(newCalibDataDoc);
                else if (graph is SiliconAOIGraph) (graph as SiliconAOIGraph).NewData(newCalibDataDoc);
                else if (graph is SiliconSaSAOIGraph) (graph as SiliconSaSAOIGraph).NewData(newCalibDataDoc);
            }
        }

        public void ClearAOIData()
        {
            foreach (var graph in AOIGraphs)
            {
                if (graph is CustomAOIGraph) (graph as CustomAOIGraph).ClearData();
                else if (graph is GDBandAOIGraph) (graph as GDBandAOIGraph).ClearData();
            }
        }

        private void RamanAOIFlowPanel_Resize(object sender, EventArgs e)
        {
            SizeGraphs();
        }

        private void SizeGraphs()
        {
            // If there is only one graph, keep it in view, otherwise keep 2 in view
            int numGraphsInView = (AOIGraphs.Count > 1) ? 2 : 1;
            numGraphsInView = AOIGraphs.Count;

            // Try to fit atleast two graphs into view at all times.
            int WidthOffset = flpGraphView.Padding.Left - flpGraphView.Padding.Right;
            int HeightOffset = flpGraphView.Padding.Top + flpGraphView.Padding.Bottom;
            if (AOIGraphs.Count > numGraphsInView)
                HeightOffset += SystemInformation.HorizontalScrollBarHeight;

            foreach (var graph in AOIGraphs)
            {
                graph.Height = flpGraphView.Height - (HeightOffset + graph.Margin.Top + graph.Margin.Bottom);
                graph.Width = (flpGraphView.Width / numGraphsInView) - (WidthOffset + graph.Margin.Top + graph.Margin.Bottom);
            }
            flpGraphView.Refresh();
        }
    }
}

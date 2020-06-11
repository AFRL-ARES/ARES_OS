using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresAdditiveAnalysisPlugin.Database.Filtering.Impl
{
    public class DataFilterOptions : BasicReactiveObjectDisposable, IDataFilterOptions
    {
        private double _blueLineStraightnessFactor;
        private bool _filterBlueLineStraightnessFactor;

        public double BlueLineStraightnessFactor
        {
            get => _blueLineStraightnessFactor;
            set => this.RaiseAndSetIfChanged(ref _blueLineStraightnessFactor, value);
        }

        public bool FilterBlueLineStraightnessFactor
        {
            get => _filterBlueLineStraightnessFactor;
            set => this.RaiseAndSetIfChanged(ref _filterBlueLineStraightnessFactor, value);
        }
    }
}

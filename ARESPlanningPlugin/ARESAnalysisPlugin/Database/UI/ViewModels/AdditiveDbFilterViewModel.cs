using AresAdditiveAnalysisPlugin.Database.Filtering;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresAdditiveAnalysisPlugin.Database.UI.ViewModels
{
    public class AdditiveDbFilterViewModel : BasicReactiveObjectDisposable
    {
        private IDataFilterOptions _filterOptions;

        public AdditiveDbFilterViewModel(IDataFilterOptions filterOptions)
        {
            FilterOptions = filterOptions;
        }

        public IDataFilterOptions FilterOptions
        {
            get => _filterOptions;
            set => this.RaiseAndSetIfChanged(ref _filterOptions, value);
        }
    }
}

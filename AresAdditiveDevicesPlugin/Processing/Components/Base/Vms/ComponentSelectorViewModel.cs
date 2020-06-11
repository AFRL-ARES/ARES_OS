using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.Events.Impl;
using ARESCore.DisposePatternHelpers;
using CommonServiceLocator;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.Processing.Components.Base.Vms
{
    public class ComponentSelectorViewModel : BasicReactiveObjectDisposable
    {
        private IComponent _selectedComponent;
        private bool _isOpen = false;
        private bool _canOpenEditor;
        private Type _selectedFilterType;
        private IComponentService _componentService;
        private ReadOnlyObservableCollection<IComponent> _availableComponents;
        private ObservableCollectionExtended<IComponent> _filteredComponents = new ObservableCollectionExtended<IComponent>();
        private readonly EventHub _eventHub;

        public ComponentSelectorViewModel(
          IComponentService componentService,
          IObservable<OpenComponentSelector> openViewListener,
          EventHub eventHub)
        {
            _eventHub = eventHub;
            _componentService = componentService;
            var availableComponentsWatcher =
              componentService
                .AllComponents
                .Connect()
                .ObserveOn(DispatcherScheduler.Current)
                .Bind(out _availableComponents)
                .Subscribe();
            var componentLoadSubscription = eventHub.GetEvent<TypedComponentsLoaded>().ObserveOnDispatcher().Subscribe(OnComponentTypeLoaded);

            CloseSelector = ReactiveCommand.CreateFromTask(async _ => await OnCloseSelector());
            OpenComponentEditor = ReactiveCommand.CreateFromTask(async _ => await OnOpenEditor());

            Disposables.Add(openViewListener.Subscribe(OnOpenEvent));
            Disposables.Add(componentLoadSubscription);
            Disposables.Add(availableComponentsWatcher);
        }

        private void OnComponentTypeLoaded(TypedComponentsLoaded componentTypeLoaded)
        {
            if (SelectedFilterType == null)
            {
                SelectedFilterType = componentTypeLoaded.ComponentType;
            }
        }

        private void OnOpenEvent(OpenComponentSelector @event)
        {

            IsComponentSelectorOpen = @event.IsOpen;
            if (!@event.IsOpen)
            {
                SelectedComponent = null;
            }

        }

        private async Task OnOpenEditor()
        {
            await OnCloseSelector();
            await _eventHub.Publish(new OpenComponentEditor(true));
        }

        private async Task OnCloseSelector()
        {
            await _eventHub.Publish(new OpenComponentSelector(false));
        }

        public IComponent SelectedComponent
        {
            get => _selectedComponent;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedComponent, value);
                CanOpenEditor = value != null;
            }
        }

        public ReadOnlyObservableCollection<Type> TypeFilters => ServiceLocator.Current.GetInstance<IComponentService>().ComponentFilterTypes;

        public bool IsComponentSelectorOpen
        {
            get => _isOpen;
            set => this.RaiseAndSetIfChanged(ref _isOpen, value);
        }

        public Type SelectedFilterType
        {
            get => _selectedFilterType;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedFilterType, value);
                FilteredComponents.Clear();
                FilteredComponents.AddRange(_availableComponents.Where(component => _selectedFilterType.IsInstanceOfType(component)));
            }
        }

        public ObservableCollectionExtended<IComponent> FilteredComponents
        {
            get => _filteredComponents;
            set => this.RaiseAndSetIfChanged(ref _filteredComponents, value);
        }

        public bool CanOpenEditor
        {
            get => _canOpenEditor;
            set => this.RaiseAndSetIfChanged(ref _canOpenEditor, value);
        }

        public ReadOnlyObservableCollection<Type> Filters => _componentService.ComponentFilterTypes;

        public ReactiveCommand<Unit, Unit> OpenComponentEditor { get; }
        public ReactiveCommand<Unit, Unit> CloseSelector { get; }
    }
}

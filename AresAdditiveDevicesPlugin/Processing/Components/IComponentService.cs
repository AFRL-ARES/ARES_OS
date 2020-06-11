using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AresAdditiveDevicesPlugin.Events.Impl;
using DynamicData;
using DynamicData.Binding;

namespace AresAdditiveDevicesPlugin.Processing.Components
{
    public interface IComponentService
    {
        IObservableList<IComponent> AllComponents { get; }
        Dictionary<ComponentPairing, IObservableCollection<IProcessData>> ComponentInputMap { get; set; }
        void Add(IComponent newComponent);
        ReadOnlyObservableCollection<Type> ComponentFilterTypes { get; }
        void LoadOpenCvComponents();
        void LoadUserDefinedComponents();
        void LoadNativeComponents();
    }
}

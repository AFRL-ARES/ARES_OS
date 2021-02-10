using DynamicData.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace ARESCore.Extensions
{
  public static class NotificationExtensions
  {
    public static IDisposable Subscribe<T, TProperty>(this T viewmodel, Expression<Func<T, TProperty>> property, Action callBack) where T : class, INotifyPropertyChanged
    {
      return Subscribe(viewmodel, property.GetPropertyInfo().Name, callBack);
    }

    public static IDisposable Subscribe<T, TProperty>(this T viewmodel, Expression<Func<T, TProperty>> property, Action<T> callBack) where T : class, INotifyPropertyChanged
    {
      return Subscribe(viewmodel, property.GetPropertyInfo().Name, callBack);
    }

    public static IDisposable Subscribe<T, TProperty>(this T viewmodel, Action<T> callBack, params Expression<Func<T, TProperty>>[] property) where T : class, INotifyPropertyChanged
    {
      return viewmodel.Subscribe(property.Select<Expression<Func<T, TProperty>>, string>(expression => expression.GetPropertyInfo().Name), callBack);
    }

    public static IDisposable Subscribe<T, TProperty>(this T viewmodel, Action callBack, params Expression<Func<T, TProperty>>[] property) where T : class, INotifyPropertyChanged
    {
      return viewmodel.Subscribe(property.Select<Expression<Func<T, TProperty>>, string>(expression => expression.GetPropertyInfo().Name), callBack);
    }

    public static IDisposable Subscribe<T, TProperty>(this T viewmodel, Expression<Func<T, TProperty>> property, Func<T, bool> where, Action<T> callBack) where T : class, INotifyPropertyChanged
    {
      return viewmodel.Subscribe(property.GetPropertyInfo().Name, where, callBack);
    }

    public static IDisposable SubscribeAndInvoke<ContainedType>(this ObservableCollection<ContainedType> collection,
      Action<ContainedType> onItemAdded, Action<ContainedType> onItemRemoved = null) where ContainedType : class
    {
      return SubscribeAndInvoke(collection as IObservableCollection<ContainedType>, onItemAdded, onItemRemoved);
    }

    // TODO: Fix this subscribe and invoke below because it just calls itself recursively forever
    public static IDisposable SubscribeAndInvoke<ContainedType>(this ObservableCollectionExtended<ContainedType> collection,
      Action<ContainedType> onItemAdded, Action<ContainedType> onItemRemoved = null) where ContainedType : class
    {
      return SubscribeAndInvoke(collection as IObservableCollection<ContainedType>, onItemAdded, onItemRemoved);
    }

    public static IDisposable SubscribeAndInvoke<ContainedType>(this IObservableCollection<ContainedType> collection, Action<ContainedType> onItemAdded, Action<ContainedType> onItemRemoved = null) where ContainedType : class
    {
      var itemsAddedDispose = Disposable.Empty;
      var itemsRemovedDispose = Disposable.Empty;

      foreach (var item in collection)
      {
        onItemAdded(item);
      }

      if (onItemAdded != null)
      {
        NotifyCollectionChangedEventHandler addedHandler = (sender, args) =>
        {
          if (args.Action != NotifyCollectionChangedAction.Add)
          {
            return;
          }

          foreach (var newItem in args.NewItems)
          {
            onItemAdded(newItem as ContainedType);
          }
        };
        collection.CollectionChanged += addedHandler;
        itemsAddedDispose = Disposable.Create(() => collection.CollectionChanged -= addedHandler);
      }

      if (onItemRemoved != null)
      {
        NotifyCollectionChangedEventHandler removedHandler = (sender, args) =>
        {
          if (args.Action != NotifyCollectionChangedAction.Remove)
          {
            return;
          }

          foreach (var newItem in args.NewItems)
          {
            onItemAdded(newItem as ContainedType);
          }
        };
        collection.CollectionChanged += removedHandler;
        itemsAddedDispose = Disposable.Create(() => collection.CollectionChanged -= removedHandler);
      }

      return Disposable.Create(() =>
      {
        itemsAddedDispose.Dispose();
        itemsRemovedDispose.Dispose();
      });
    }
    public static IDisposable SubscribeAndInvoke<T, TProperty>(this T viewmodel, Expression<Func<T, TProperty>> property, Action callBack) where T : class, INotifyPropertyChanged
    {
      return SubscribeAndInvoke(viewmodel, property.GetPropertyInfo().Name, callBack);
    }

    public static IDisposable SubscribeAndInvoke<T, TProperty>(this T viewmodel, Expression<Func<T, TProperty>> property, Action<T> callBack) where T : class, INotifyPropertyChanged
    {
      return SubscribeAndInvoke(viewmodel, property.GetPropertyInfo().Name, callBack);
    }

    public static IDisposable SubscribeAndInvoke<T, TProperty>(this T viewmodel, Expression<Func<T, TProperty>> property, Func<T, bool> where, Action<T> callBack) where T : class, INotifyPropertyChanged
    {
      return viewmodel.SubscribeAndInvoke(property.GetPropertyInfo().Name, where, callBack);
    }

    public static IDisposable SubscribeAndInvoke<T, TProperty>(this T viewmodel, Action<T> callBack, params Expression<Func<T, TProperty>>[] property) where T : class, INotifyPropertyChanged
    {
      return viewmodel.SubscribeAndInvoke(property.Select<Expression<Func<T, TProperty>>, string>(expression => expression.GetPropertyInfo().Name), callBack);
    }

    public static IDisposable SubscribeAndInvoke<T, TProperty>(this T viewmodel, Action callBack, params Expression<Func<T, TProperty>>[] property) where T : class, INotifyPropertyChanged
    {
      return viewmodel.SubscribeAndInvoke(property.Select<Expression<Func<T, TProperty>>, string>(expression => expression.GetPropertyInfo().Name), callBack);
    }

    public static IDisposable Map<T, TProperty>(this T viewmodel, Expression<Func<T, TProperty>> property, Func<T, IDisposable> callBack, Func<T, bool> filter = null) where T : class, INotifyPropertyChanged
    {
      filter = filter ?? (item => true);
      var repeatDisposable = new SimpleDisposable();
      var handler = viewmodel.SubscribeAndInvoke(property, item =>
     {
       repeatDisposable.Dispose();
       if (filter(item))
         repeatDisposable.Add(callBack(item));
     });
      return new SimpleDisposable(handler, repeatDisposable);
    }

    public static IDisposable MapAsync<T, TProperty>(this T viewmodel, Expression<Func<T, TProperty>> property, Func<T, Task<IDisposable>> callBack, Func<T, bool> filter = null) where T : class, INotifyPropertyChanged
    {
      filter = filter ?? (item => true);
      var repeatDisposable = new SimpleDisposable();
      var handler = viewmodel.SubscribeAndInvoke(property, async item =>
     {
       repeatDisposable.Dispose();
       if (filter(item))
         repeatDisposable.Add(await callBack(item));
     });
      return new SimpleDisposable(handler, repeatDisposable);
    }

    private static IDisposable Subscribe<T>(this T viewmodel, string propertyName, Action callBack) where T : class, INotifyPropertyChanged
    {
      PropertyChangedEventHandler handler = (sender, args) =>
      {
        if (args.PropertyName == propertyName)
          callBack();
      };
      viewmodel.PropertyChanged += handler;

      return Disposable.Create(() => viewmodel.PropertyChanged -= handler);
    }

    private static IDisposable Subscribe<T>(this T viewmodel, string propertyName, Action<T> callBack) where T : class, INotifyPropertyChanged
    {
      PropertyChangedEventHandler handler = (sender, args) =>
      {
        if (args.PropertyName == propertyName)
          callBack(viewmodel);
      };
      viewmodel.PropertyChanged += handler;

      return Disposable.Create(() => viewmodel.PropertyChanged -= handler);
    }

    private static IDisposable Subscribe<T>(this T viewmodel, IEnumerable<string> propertyNames, Action callBack) where T : class, INotifyPropertyChanged
    {
      PropertyChangedEventHandler handler = (sender, args) =>
      {
        if (propertyNames.Contains(args.PropertyName))
          callBack();
      };
      viewmodel.PropertyChanged += handler;

      return Disposable.Create(() => viewmodel.PropertyChanged -= handler);
    }

    private static IDisposable Subscribe<T>(this T viewmodel, IEnumerable<string> propertyNames, Action<T> callBack) where T : class, INotifyPropertyChanged
    {
      PropertyChangedEventHandler handler = (sender, args) =>
      {
        if (propertyNames.Contains(args.PropertyName))
          callBack(viewmodel);
      };
      viewmodel.PropertyChanged += handler;

      return Disposable.Create(() => viewmodel.PropertyChanged -= handler);
    }

    private static IDisposable Subscribe<T>(this T viewmodel, string propertyName, Func<T, bool> where, Action<T> callBack) where T : class, INotifyPropertyChanged
    {
      PropertyChangedEventHandler handler = (sender, args) =>
      {
        if (args.PropertyName == propertyName && where(viewmodel))
          callBack(viewmodel);
      };
      viewmodel.PropertyChanged += handler;

      return Disposable.Create(() => viewmodel.PropertyChanged -= handler);
    }

    private static IDisposable SubscribeAndInvoke<T>(this T viewmodel, string propertyName, Action callBack) where T : class, INotifyPropertyChanged
    {
      PropertyChangedEventHandler handler = (sender, args) =>
      {
        if (args.PropertyName == propertyName)
          callBack();
      };
      viewmodel.PropertyChanged += handler;
      callBack();

      return Disposable.Create(() => viewmodel.PropertyChanged -= handler);
    }

    private static IDisposable SubscribeAndInvoke<T>(this T viewmodel, string propertyName, Action<T> callBack) where T : class, INotifyPropertyChanged
    {
      PropertyChangedEventHandler handler = (sender, args) =>
      {
        if (args.PropertyName == propertyName)
          callBack(viewmodel);
      };
      viewmodel.PropertyChanged += handler;
      callBack(viewmodel);

      return Disposable.Create(() => viewmodel.PropertyChanged -= handler);
    }

    private static IDisposable SubscribeAndInvoke<T>(this T viewmodel, string propertyName, Func<T, bool> where, Action<T> callBack) where T : class, INotifyPropertyChanged
    {
      PropertyChangedEventHandler handler = (sender, args) =>
      {
        if (args.PropertyName == propertyName && where(viewmodel))
          callBack(viewmodel);
      };
      viewmodel.PropertyChanged += handler;
      callBack(viewmodel);

      return Disposable.Create(() => viewmodel.PropertyChanged -= handler);
    }

    private static IDisposable SubscribeAndInvoke<T>(this T viewmodel, IEnumerable<string> propertyNames, Action callBack) where T : class, INotifyPropertyChanged
    {
      PropertyChangedEventHandler handler = (sender, args) =>
      {
        if (propertyNames.Contains(args.PropertyName))
          callBack();
      };
      viewmodel.PropertyChanged += handler;
      callBack();

      return Disposable.Create(() => viewmodel.PropertyChanged -= handler);
    }

    private static IDisposable SubscribeAndInvoke<T>(this T viewmodel, IEnumerable<string> propertyNames, Action<T> callBack) where T : class, INotifyPropertyChanged
    {
      PropertyChangedEventHandler handler = (sender, args) =>
      {
        if (propertyNames.Contains(args.PropertyName))
          callBack(viewmodel);
      };
      viewmodel.PropertyChanged += handler;
      callBack(viewmodel);

      return Disposable.Create(() => viewmodel.PropertyChanged -= handler);
    }
  }
}

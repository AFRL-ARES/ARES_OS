using System;
using System.Collections.Generic;
using System.Linq;

namespace AresAdditiveDevicesPlugin.Extensions
{
  public static class CollectionExtensions
  {
    public static ICollection<T> AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
      if (collection == null)
        throw new ArgumentNullException(nameof(collection));
      if (items == null)
        throw new ArgumentNullException(nameof(items));
      foreach (var obj in items)
        collection.Add(obj);
      return collection;
    }

    public static int RemoveWhere<TSource>(
        this ICollection<TSource> source,
        Func<TSource, bool> predicate)
    {
      if (source == null)
        throw new ArgumentNullException(nameof(source));
      if (predicate == null)
        throw new ArgumentNullException(nameof(predicate));

      var itemsToRemove = source.Where(predicate).ToList();
      itemsToRemove.ForEach(item => source.Remove(item));

      return itemsToRemove.Count;
    }

    //    public static IDisposable SubscribeAndInvoke<ContainedType>(this IObservableCollection<ContainedType> collection, Action<ContainedType> onItemAdded, Action<ContainedType> onItemRemoved = null) where ContainedType : class
    //    {
    //      var itemsAddedDispose = Disposable.Empty;
    //      var itemsRemovedDispose = Disposable.Empty;
    //
    //      collection.ForEach(onItemAdded);
    //
    //      if (onItemAdded != null)
    //      {
    //        NotifyCollectionChangedEventHandler addedHandler = (sender, args) =>
    //        {
    //          if (args.Action != NotifyCollectionChangedAction.Add)
    //          {
    //            return;
    //          }
    //          args.NewItems.ForEach(onItemAdded);
    //        };
    //        collection.CollectionChanged += addedHandler;
    //        itemsAddedDispose = Disposable.Create(() => collection.CollectionChanged -= addedHandler);
    //      }
    //
    //      if (onItemRemoved != null)
    //      {
    //        NotifyCollectionChangedEventHandler removedHandler = (sender, args) =>
    //        {
    //          if (args.Action != NotifyCollectionChangedAction.Remove)
    //          {
    //            return;
    //          }
    //          args.NewItems.ForEach(onItemAdded);
    //        };
    //        collection.CollectionChanged += removedHandler;
    //        itemsAddedDispose = Disposable.Create(() => collection.CollectionChanged -= removedHandler);
    //      }
    //
    //      return Disposable.Create(() =>
    //      {
    //        itemsAddedDispose.Dispose();
    //        itemsRemovedDispose.Dispose();
    //      });
    //    }
  }
}

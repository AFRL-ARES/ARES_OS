using ARESCore.DeviceSupport;
using DynamicData.Binding;
using NationalInstruments.Restricted;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ARESCore.Extensions
{
  public static class DeviceExtensions
  {
    public static IDisposable MonitorCurrentState(this IAresDevice aresDevice)
    {
      return aresDevice.CurrentState.WhenAnyPropertyChanged().Subscribe(deviceState =>
     {
       var deviceStateProperties = GetStatePropertiesRecursively(deviceState);
       var logEntry = string.Empty;

       deviceStateProperties.ForEach(stateProperty =>
       {
          // Could wourk with well defined CSV mappings
          var entryName = stateProperty.Value.Name;
          //          var entryValue = typeof( IAresDeviceState ).IsAssignableFrom( stateProperty.Value.PropertyType ) ? $":" : $" = {stateProperty.Value.GetValue( stateProperty.Key )}";
          var entryValue = $"{stateProperty.Value.GetValue(stateProperty.Key)}";
         logEntry += $"{entryName} = {entryValue}\n";
       });

        // TODO FIXME
        // dbHistory.PublishDocAsync( new LogDatabaseEntry( logEntry ) );
      });
    }

    private static IEnumerable<KeyValuePair<IAresDeviceState, PropertyInfo>> GetStatePropertiesRecursively(IAresDeviceState aresDeviceState)
    {
      var deviceStateProperties = aresDeviceState.GetType().GetProperties().Where(property => property.DeclaringType == aresDeviceState.GetType());
      foreach (var stateProperty in deviceStateProperties)
      {
        // PropertyInfo<IAresDeviceState[]> stateCollectionProperties
        var isCollection = typeof(IEnumerable<IAresDeviceState>).IsAssignableFrom(stateProperty.DeclaringType);
        if (isCollection)
        {
          // IAresDeviceStates[]
          var statePropertyStateEnumerable = stateProperty.GetValue(aresDeviceState).MakeEnumerable().Cast<IAresDeviceState>();
          // IAresDeviceState
          foreach (var statePropertyState in statePropertyStateEnumerable)
          {
            // IEnumerable<KeyValuePair<IAresState, PropertyInfo>>    ---> code already yields, no need to iterate through
            var statePropertyCollection = GetStatePropertiesRecursively(statePropertyState);
            //            foreach ( var statePropertyCollectionState in statePropertyCollection )
            //            {
            //              yield return statePropertyCollectionState;
            //            }
          }
        }

        // The current property is another IAresDeviceState
        if (typeof(IAresDeviceState).IsAssignableFrom(stateProperty.PropertyType))
        {

          var recursedDeviceState = stateProperty.GetValue(aresDeviceState) as IAresDeviceState;
          //          var recursedDeviceStateProperties = GetStatePropertiesRecursively( recursedDeviceState );
          yield return new KeyValuePair<IAresDeviceState, PropertyInfo>(aresDeviceState, stateProperty);
          foreach (var recursedDeviceStateProperty in GetStatePropertiesRecursively(recursedDeviceState))
          {
            yield return recursedDeviceStateProperty;
          }
        }
        else
        {
          yield return new KeyValuePair<IAresDeviceState, PropertyInfo>(aresDeviceState, stateProperty);
        }
      }
    }
  }
}
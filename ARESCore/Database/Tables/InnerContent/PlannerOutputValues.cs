using System;
using System.Collections.Generic;
using System.Linq;
using DynamicData.Annotations;

namespace ARESCore.Database.Tables.InnerContent
{
  public class PlannerOutputValues
  {
   [CanBeNull]
    public string InternalDescs { get; set; }
    public string[] Descs
    {
      get => InternalDescs.Split( ';' );
      set => InternalDescs = string.Join( ";", value );
    }

    [CanBeNull]
    public string InternalData { get; set; }

    public List<double[]> Data
    {
      get
      {
        var arr = InternalData.Split( '|' );
        var toReturn = new List<double[]>();
        foreach ( var entryArray in  arr)
        {
          toReturn.Add(Array.ConvertAll( entryArray.Split( ';' ), v => double.Parse( v ) ));
        }
        return toReturn;
      }
      set
      {
        InternalData = "";
        foreach ( var doubles in value )
        {
          InternalData += String.Join( ";", doubles.Select( p => p.ToString() ).ToArray() );
          InternalData += "|";
        }
        InternalData = InternalData.Substring( 0, InternalData.Length - 1 );

      }
    }
  }
}

using System;
using System.Linq;
using DynamicData.Annotations;

namespace ARESCore.Database.Tables.InnerContent
{
  public class ExperimentData
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

    public double[] Data
    {
      get { return Array.ConvertAll( InternalData.Split( ';' ), v => double.Parse( v ) ); }
      set { InternalData = string.Join( ";", value.Select( p => p.ToString() ).ToArray() ); }
    }
  }
}
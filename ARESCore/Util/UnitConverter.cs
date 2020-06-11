using System;
using System.Collections.Generic;
using System.Linq;

namespace ARESCore.Util
{
  public class UnitConverter
  {
    public static double Convert( double val, Power @from, Power to )
    {
      if ( @from == to )
        return val;

      switch ( @from )
      {
        case Power.Watt:
        {
          switch ( to )
          {
            case Power.Milliwatt:
              return val * 1000.0;
            case Power.Kilowatt:
              return val* 0.001;
          }
        }
          break;
        case Power.Milliwatt:
        {
          switch ( to )
          {
            case Power.Watt:
              return val * 0.001;
            case Power.Kilowatt:
              return val * 0.000001;
          }
        }
          break;
        case Power.Kilowatt:
        {
          switch ( to )
          {
            case Power.Watt:
              return val * 1000.0;
            case Power.Milliwatt:
              return val * 1000000.0;
          }
        }
          break;
        default:
          throw new NotImplementedException( "[double.GetConversionFunction]: Cannot convert from " + @from.ToString("g") + " to " + to.ToString("g") );
      }

      return val;
    }
    public static double Convert(double val, Length @from, Length to )
    {
      if ( @from == to )
        return val;

      switch ( @from )
      {
        case Length.Meter:
        {
          switch ( to )
          {
            case Length.Centimeter:
              return val * 100.0;
            case Length.Millimeter:
              return val * 1000.0;
            case Length.Micrometer:
              return val * 1000000.0;
            case Length.Nanometer:
              return val * 1000000000.0;
          }
        }
          break;
        case Length.Centimeter:
        {
          switch ( to )
          {
            case Length.Meter:
              return val * 0.01;
            case Length.Millimeter:
              return val * 10.0;
            case Length.Micrometer:
              return val * 10000.0;
            case Length.Nanometer:
              return val * 10000000.0;
          }
        }
          break;
        case Length.Millimeter:
        {
          switch ( to )
          {
            case Length.Meter:
              return val * 0.001;
            case Length.Centimeter:
              return val * 0.1;
            case Length.Micrometer:
              return val * 1000.0;
            case Length.Nanometer:
              return val * 1000000.0;
          }
        }
          break;
        case Length.Micrometer:
        {
          switch ( to )
          {
            case Length.Meter:
              return val * 0.000001;
            case Length.Centimeter:
              return val * 0.0001;
            case Length.Millimeter:
              return val * 0.001;
            case Length.Nanometer:
              return val * 1000;
          }
        }
          break;
        case Length.Nanometer:
        {
          switch ( to )
          {
            case Length.Meter:
              return val * 0.000000001;
            case Length.Centimeter:
              return val * 0.0000001;
            case Length.Millimeter:
              return val * 0.000001;
            case Length.Micrometer:
              return val * 0.001;
          }
        }
          break;
        default:
          throw new NotImplementedException( "[double.GetConversionFunction]: Cannot convert from " + @from.ToString("g") + " to " + to.ToString("g") );
      }

      return val;
    }
    public static double Convert(double val, FlowRate fromRate, FlowRate toRate )
    {
      if ( fromRate == toRate )
        return val;

      switch ( fromRate )
      {
        case FlowRate.SCCM:
        {
          switch ( toRate )
          {
            case FlowRate.SLPM:
              return val / 1000.0;
            default:
              throw new NotImplementedException( "[double.GetConversionFunction]: Cannot convert from " + fromRate.ToString("g") + " to " + toRate.ToString("g") );
          }
        }
        case FlowRate.SLPM:
        {
          switch ( toRate )
          {
            case FlowRate.SCCM:
              return val * 1000.0;
            default:
              throw new NotImplementedException( "[double.GetConversionFunction]: Cannot convert from " + fromRate.ToString("g") + " to " + toRate.ToString("g") );
          }
        }
        case FlowRate.ULMIN:
        {
          switch ( toRate )
          {
            case FlowRate.ULHR:
              return val * 60.0;
            case FlowRate.MLMIN:
              return val * 0.001;
            case FlowRate.MLHR:
              return val * 0.06;
            default:
              throw new NotImplementedException( "[double.GetConversionFunction]: Cannot convert from " + fromRate.ToString("g") + " to " + toRate.ToString("g") );
          }
        }
        case FlowRate.ULHR:
        {
          switch ( toRate )
          {
            case FlowRate.ULMIN:
              return val / 60.0;
            case FlowRate.MLMIN:
              return val / 60000.0;
            case FlowRate.MLHR:
              return val * 0.001;
            default:
              throw new NotImplementedException( "[double.GetConversionFunction]: Cannot convert from " + fromRate.ToString("g") + " to " + toRate.ToString("g") );
          }
        }
        case FlowRate.MLMIN:
        {
          switch ( toRate )
          {
            case FlowRate.ULMIN:
              return val * 1000.0;
            case FlowRate.ULHR:
              return val * 60000.0;
            case FlowRate.MLHR:
              return val * 60.0;
            default:
              throw new NotImplementedException( "[double.GetConversionFunction]: Cannot convert from " + fromRate.ToString("g") + " to " + toRate.ToString("g") );
          }
        };
        case FlowRate.MLHR:
        {
          switch ( toRate )
          {
            case FlowRate.ULMIN:
              return val * ( 1000.0 / 60.0);
            case FlowRate.ULHR:
              return val * 1000.0;
            case FlowRate.MLMIN:
              return val / 60.0;
            default:
              throw new NotImplementedException( "[double.GetConversionFunction]: Cannot convert from " + fromRate.ToString("g") + " to " + toRate.ToString("g") );
          }
        }
        default:
          throw new NotImplementedException( "[double.GetConversionFunction]: Cannot convert from " + fromRate.ToString("g") + " to " + toRate.ToString("g") );
      }
    }
    public static double Convert(double val, Pressure @from, Pressure to )
    {
      if ( @from == to )
        return val;

      switch ( @from )
      {
        case Pressure.Torr:
        {
          switch ( to )
          {
            case Pressure.Pascal:
              return val * 133.322;
            case Pressure.Atmosphere:
              return val * 0.0013157858376511;
            case Pressure.InHg:
              return val * 0.0393701;
          }
        }
          break;
        case Pressure.Pascal:
        {
          switch ( to )
          {
            case Pressure.Torr:
              return val * 0.0075006375541921;
            case Pressure.Atmosphere:
              return val * 0.0000098692326671601;
            case Pressure.InHg:
              return val * 0.0002953;
          }
        }
          break;
        case Pressure.Atmosphere:
        {
          switch ( to )
          {
            case Pressure.Torr:
              return val * 760.00210017852;
            case Pressure.Pascal:
              return val * 101325;
            case Pressure.InHg:
              return val * 0.033421;
          }
        }
          break;
        case Pressure.InHg:
        {
          switch ( to )
          {
            case Pressure.Torr:
              return val * 25.399904;
            case Pressure.Pascal:
              return val * 3386.375258;
            case Pressure.Atmosphere:
              return val * 29.921373;
          }
        }
          break;
        default:
          throw new NotImplementedException( "[double.GetConversionFunction]: Cannot convert from " + @from.ToString("g") + " to " + to.ToString("g") );
      }

      return val;
    }
    public static double Convert(double val, Temperature @from, Temperature to )
    {
      if ( @from == to )
        return val;

      switch ( @from )
      {
        case Temperature.Kelvin:
        {
          switch ( to )
          {
            case Temperature.Celcius:
              return val - 273.15;
          }
        }
          break;
        case Temperature.Celcius:
        {
          switch ( to )
          {
            case Temperature.Kelvin:
              return val + 273.15;
          }
        }
          break;
        default:
          throw new NotImplementedException( "[double.GetConversionFunction]: Cannot convert from " + @from.ToString("g") + " to " + to.ToString("g") );
      }

      return val;
    }
    public static double Convert(double val, Concentration @from, Concentration to )
    {
      return val;
    }
    public static double Convert(double val, Volume @from, Volume to )
    {
      if ( @from == to )
        return val;

      switch ( @from )
      {
        case Volume.Milliliters:
        {
          switch ( to )
          {
            case Volume.Microliters:
              return val * 1000.0;
          }
        }
          break;
        case Volume.Microliters:
        {
          switch ( to )
          {
            case Volume.Milliliters:
              return val / 1000.0;
          }
        }
          break;
        default:
          throw new NotImplementedException( "[ARESConcentration.GetConversionFunction]: Cannot convert from " + @from.ToString("g") + " to " + to.ToString("g") );
      }

      return val;
    }
    public static double Convert(double val, ElectricPotential @from, ElectricPotential to )
    {
      return val;
    }
  }

}
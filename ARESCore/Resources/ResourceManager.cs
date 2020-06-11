namespace ARESCore.Resources
{
  public class ResourceManager : IResourceManager
  {
    public string GetString( string inputString )
    {
      if ( inputString.Equals( "Shutter (Lambda 10-3)" ) ) return "Shutter (Lambda 10-3)";
      if ( inputString.Equals( "Malformed DataFrame: " ) ) return "Malformed DataFrame: ";
      if ( inputString.Equals( "Water Meter (Shaw SuperDew)" ) ) return "Water Meter (Shaw SuperDew)";
      if ( inputString.Equals( "WriteFrame function passed null frame!" ) ) return "WriteFrame function passed null frame!";
      if ( inputString.Equals( " DataFrame unparseable!" ) ) return " DataFrame unparseable!";
      if ( inputString.Equals( " DataFrame.Data is too short!" ) ) return " DataFrame.Data is too short!";
      if ( inputString.Equals( "ReadFrame function passed wrong frame type!" ) ) return "ReadFrame function passed wrong frame type!";
      if ( inputString.Equals( "CCD (Newton)" ) ) return "CCD (Newton)";
      if ( inputString.Equals( "Microscope (Nikon Ti)" ) ) return "Microscope (Nikon Ti)";
      if ( inputString.Equals( "Water Valve (National Instruments)" ) ) return "Water Valve (National Instruments)";
      if ( inputString.Equals( "ReadFrame.USB call did not complete successfully!" ) ) return "ReadFrame.USB call did not complete successfully!";
      if ( inputString.Equals( "WriteFrame.USB call did not complete successfully!" ) ) return "WriteFrame.USB call did not complete successfully!";
      if ( inputString.Equals( "WaterCEM (Bronkhorst)" ) ) return "WaterCEM (Bronkhorst)";
      return "";
    }
  }
}
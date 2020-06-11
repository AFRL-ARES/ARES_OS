using System;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ARESCore.TMPDbMigration.Migrators
{
  public class PointJsonConverter : JsonConverter
  {
    public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
    {
      throw new NotImplementedException();
    }

    public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
    {
      var jPoint = JObject.Load( reader );

      var x = jPoint["X"].Value<int>();
      var y = jPoint["Y"].Value<int>();
      return new Point( x, y );
    }

    public override bool CanConvert( Type objectType )
    {
      return objectType == typeof( Point );
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ARESCore.Util
{
  public class JsonTypeConverter<T> : JsonConverter
  {
    public override bool CanConvert( Type objectType ) => true;

    public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
    {
      return serializer.Deserialize<T>( reader );
    }

    public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
    {
      serializer.Serialize( writer, value );
    }
  }
}
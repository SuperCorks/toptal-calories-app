using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calories.App.Serialization
{
    /// <summary>
    /// Custom <see cref="JsonConverter"/> that makes sure all <see cref="DateTime"/> properties are serialized
    /// into UTC ISO date time format and deserialized into local time DateTime.
    /// </summary>
    /// <see cref="https://www.newtonsoft.com/json/help/html/DatesInJSON.htm"/>
    class JsonDateTimeConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(DateTime) || typeToConvert == typeof(Nullable<DateTime>);
        }

        public override object ReadJson(JsonReader reader, Type targetType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            if (reader.TokenType == JsonToken.Date) return ((DateTime)reader.Value).ToLocalTime();

            throw new Exception("Cannot deserialize a date that isn't a string!");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null) writer.WriteNull();
            else writer.WriteValue(((DateTime)value).ToUniversalTime().ToString("o")); // ISO format (UTC always));
        }
    }
}

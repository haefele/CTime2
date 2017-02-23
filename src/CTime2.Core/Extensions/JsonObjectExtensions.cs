using System;
using System.Reflection;
using Windows.Data.Json;

namespace CTime2.Core.Extensions
{
    public static class JsonObjectExtensions
    {
        public static string GetString(this JsonObject self, string name)
        {
            var value = self.GetNamedValue(name, JsonValue.CreateNullValue());

            return value.ValueType == JsonValueType.Null 
                ? null 
                : value.GetString();
        }

        public static byte[] GetBase64ByteArray(this JsonObject self, string name)
        {
            return Convert.FromBase64String(self.GetString(name) ?? string.Empty);
        }

        public static int GetInt(this JsonObject self, string name)
        {
            return (int)self.GetNamedNumber(name, 0);
        }

        public static DateTime GetDateTime(this JsonObject self, string name)
        {
            return DateTime.Parse(self.GetString(name));
        }

        public static T? GetNullableEnum<T>(this JsonObject self, string name)
            where T : struct
        {
            if (typeof(T).GetTypeInfo().IsEnum == false)
                throw new Exception($"{typeof(T)} is no enum.");

            var value = self.GetNamedValue(name, JsonValue.CreateNullValue());

            if (value.ValueType == JsonValueType.Null)
                return null;

            if (value.ValueType == JsonValueType.Number)
            {
                var i = (int) value.GetNumber();
                return (T)Enum.ToObject(typeof(T), i);
            }

            if (value.ValueType == JsonValueType.String)
            {
                var s = value.GetString();
                if (Enum.IsDefined(typeof(T), s))
                {
                    return (T) Enum.Parse(typeof(T), s);
                }
            }

            return null;
        }

        public static int? GetNullableInt(this JsonObject self, string name)
        {
            var value = self.GetNamedValue(name, JsonValue.CreateNullValue());

            return value.ValueType == JsonValueType.Null
                ? (int?)null
                : self.GetInt(name);
        }

        public static DateTime? GetNullableDateTime(this JsonObject self, string name)
        {
            var value = self.GetNamedValue(name, JsonValue.CreateNullValue());

            return value.ValueType == JsonValueType.Null
                ? (DateTime?)null
                : self.GetDateTime(name);
        }

        public static TimeSpan GetTimeSpan(this JsonObject self, string name)
        {
            var time = self.GetString(name);

            if (string.IsNullOrWhiteSpace(time))
                return TimeSpan.Zero;
            
            string[] parts = time.Split(':');

            int hours = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);

            return new TimeSpan(hours, minutes, 0);
        }
    }
}
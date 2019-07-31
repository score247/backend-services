namespace Score247.Shared.Extensions
{
    using System;
    using System.Data;
    using Dapper;
    using Newtonsoft.Json;

    public class JsonTypeHandler : SqlMapper.ITypeHandler
    {
        public void SetValue(IDbDataParameter parameter, object value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }

        public object Parse(Type destinationType, object value)
        {
            return JsonConvert.DeserializeObject(value as string, destinationType);
        }

        public static string SerializeObjectWithUtcSetting(object obj)
            => obj == null
            ? "null"
            : JsonConvert.SerializeObject(
                obj,
                new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });
    }
}
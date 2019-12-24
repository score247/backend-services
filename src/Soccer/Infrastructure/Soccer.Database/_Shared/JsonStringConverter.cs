using Newtonsoft.Json;

namespace Soccer.Database._Shared
{
    public static class JsonStringConverter
    {
        public static string ToJsonString(object obj)
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

namespace Soccer.Database
{
    using Fanex.Data.Repository;
    using Newtonsoft.Json;

    public abstract class BaseCommand : NonQueryCommand
    {
        protected string ToJsonString(object obj)
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
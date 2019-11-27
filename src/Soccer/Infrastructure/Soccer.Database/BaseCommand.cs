using System;
using Fanex.Data;
using Fanex.Data.Repository;
using Newtonsoft.Json;

namespace Soccer.Database
{
    public abstract class BaseCommand : NonQueryCommand
    {
        protected BaseCommand(DateTimeOffset eventDate = default) 
        {
            EventDate = eventDate == default ? DateTimeOffset.Now : eventDate;
        }

        [SpParam(Ignored = true)]
        protected DateTimeOffset EventDate { get; }

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
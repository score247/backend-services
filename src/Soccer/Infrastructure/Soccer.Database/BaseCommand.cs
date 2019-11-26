using System;
using Fanex.Data;
using Fanex.Data.Repository;
using Newtonsoft.Json;

namespace Soccer.Database
{
    public abstract class BaseCommand : NonQueryCommand
    {
        protected BaseCommand(DateTime eventDate = default) 
        {
            EventDate = eventDate == default ? DateTime.Now : eventDate;
        }

        [SpParam(Ignored = true)]
        protected DateTime EventDate { get; }

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
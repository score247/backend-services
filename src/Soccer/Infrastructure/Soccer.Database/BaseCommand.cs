using System;
using Fanex.Data;
using Fanex.Data.Repository;
using Soccer.Database._Shared;

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

        protected static string ToJsonString(object obj) => JsonStringConverter.ToJsonString(obj);
    }
}
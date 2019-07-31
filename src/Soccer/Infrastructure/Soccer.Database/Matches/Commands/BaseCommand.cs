namespace Soccer.Database.Matches.Commands
{
    using System;
    using Fanex.Data.Repository;
    using Score247.Shared.Extensions;

    public abstract class BaseCommand : NonQueryCommand
    {
        protected BaseCommand(Func<object, string> jsonConvert)
        {
            ToJsonString = jsonConvert ?? JsonTypeHandler.SerializeObjectWithUtcSetting;
        }

        protected BaseCommand() : this(null)
        {
        }

        protected Func<object, string> ToJsonString { get; }
    }
}
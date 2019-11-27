using System;
using Microsoft.Extensions.Logging;

namespace Soccer.DataReceivers.ScheduleTasks._Shared.Helpers
{
    public class HangfireLogger : ILogger
    {
        private readonly string name;
        private readonly Fanex.Logging.ILogger logger;

        public HangfireLogger(string name, Fanex.Logging.ILogger logger = null)
        {
            this.name = name;
            this.logger = logger ?? Fanex.Logging.Logger.Log;
        }

        public IDisposable BeginScope<TState>(TState state)
            => null;

        public bool IsEnabled(LogLevel logLevel)
            => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logLevel > LogLevel.Information)
            {
                var message = $"{name} {state} {exception?.ToString()}";

                if (logLevel == LogLevel.Warning)
                {
                    logger.Warn(message);
                }
                else
                {
                    logger.Error(message);
                }
            }
        }
    }
}
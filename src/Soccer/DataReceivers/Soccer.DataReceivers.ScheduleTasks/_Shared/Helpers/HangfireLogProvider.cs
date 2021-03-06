using Microsoft.Extensions.Logging;

namespace Soccer.DataReceivers.ScheduleTasks._Shared.Helpers
{
#pragma warning disable S3881 // "IDisposable" should be implemented correctly

    public class HangfireLogProvider : ILoggerProvider
#pragma warning restore S3881 // "IDisposable" should be implemented correctly
    {
        private ILogger logger;

        public ILogger CreateLogger(string categoryName)
            => logger ??= new HangfireLogger(categoryName);

        public void Dispose()
        {
            logger = null;
        }
    }
}
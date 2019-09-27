using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Logging;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Soccer.Core.Matches.QueueMessages;
using Soccer.EventPublishers.Hubs;

namespace Soccer.EventPublishers.Matches
{
    public class UpdateLiveMatchPublisher : IConsumer<ILiveMatchUpdatedMessage>
    {
        private readonly IHubContext<SoccerHub> hubContext;
        private readonly ILogger logger;

        public UpdateLiveMatchPublisher(IHubContext<SoccerHub> hubContext, ILogger logger)
        {
            this.hubContext = hubContext;
            this.logger = logger;
        }

        public Task Consume(ConsumeContext<ILiveMatchUpdatedMessage> context)
        {
            var message = context.Message;

            if (message != null)
            {
                var newMatches = message.NewMatches;
                var removedMatches = message.RemovedMatches;
            }
            throw new NotImplementedException();
        }
    }
}
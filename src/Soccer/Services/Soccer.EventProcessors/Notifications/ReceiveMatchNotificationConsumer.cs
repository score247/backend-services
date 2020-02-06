using System.Threading.Tasks;
using MassTransit;
using Soccer.Core.Notification.Models;
using Soccer.Core.Notification.QueueMessages;

namespace Soccer.EventProcessors.Notifications
{
    public class ReceiveMatchNotificationConsumer : IConsumer<IMatchNotificationReceivedMessage>
    {
        private readonly IBus messageBus;

        public ReceiveMatchNotificationConsumer(IBus messageBus) 
        {
            this.messageBus = messageBus;
        }

        public async Task Consume(ConsumeContext<IMatchNotificationReceivedMessage> context)
        {
            var message = context.Message;

            //TODO generate notification message
            

            //TODO de-dup message

            //TODO publish process notifications

            //TODO language translation

            await messageBus.Publish<IMatchNotificationProcessedMessage>(new MatchNotificationProcessedMessage(
                new MatchEventNotification(
                    message.MatchId,
                    $"{message.Timeline.Type.DisplayName.Replace('_', ' ').ToUpperInvariant()}",
                    $"Home {message.MatchResult?.HomeScore} : Away {message.MatchResult?.AwayScore}",
                    new string[]{ }
                )));
        }
    }
}

namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using System.Threading.Tasks;
    using MassTransit;
    using Soccer.Core.Matches.QueueMessages;

    public interface IIncreaseMatchTimeTask
    {
        Task IncreaseMatchTime();
    }

    public class IncreaseMatchTimeTask : IIncreaseMatchTimeTask
    {
        private readonly IBus messageBus;

        public IncreaseMatchTimeTask(IBus messageBus)
        {
            this.messageBus = messageBus;
        }

        public async Task IncreaseMatchTime()
        {
            await messageBus.Publish<IMatchTimeIncreasedMessage>(new MatchTimeIncreasedMessage());
        }
    }
}
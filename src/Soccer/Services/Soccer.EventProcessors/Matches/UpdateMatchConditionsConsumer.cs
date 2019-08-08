namespace Soccer.EventProcessors.Matches
{
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Database.Matches.Commands;
    using System.Threading.Tasks;

    public class UpdateMatchConditionsConsumer : IConsumer<IMatchUpdatedConditionsMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public UpdateMatchConditionsConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IMatchUpdatedConditionsMessage> context)
        {
            var message = context.Message;

            var command = new UpdateMatchRefereeAndAttendanceCommand(message.MatchId, message.Referee, message.Attendance, message.Language.DisplayName);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}

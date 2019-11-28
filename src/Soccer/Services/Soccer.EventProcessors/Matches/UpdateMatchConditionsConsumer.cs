namespace Soccer.EventProcessors.Matches
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Database.Matches.Commands;

    public class UpdateMatchConditionsConsumer : IConsumer<IMatchUpdatedConditionsMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public UpdateMatchConditionsConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public Task Consume(ConsumeContext<IMatchUpdatedConditionsMessage> context)
        {
            var message = context.Message;

            var command = new UpdateMatchRefereeAndAttendanceCommand(
                message.MatchId, 
                message.Referee, 
                message.Attendance, 
                message.Language.DisplayName, 
                message.EventDate);

            return dynamicRepository.ExecuteAsync(command);
        }
    }
}

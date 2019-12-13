using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Database.Matches.Commands;

namespace Soccer.EventProcessors.Matches
{
    public class UpdateMatchConditionsConsumer : IConsumer<IMatchUpdatedConditionsMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public UpdateMatchConditionsConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IMatchUpdatedConditionsMessage> context)
        {
            if (context.Message == null || context.Message.Language == null)
            {
                return;
            }

            var command = new UpdateMatchRefereeAndAttendanceCommand(
                context.Message.MatchId,
                context.Message.Referee,
                context.Message.Attendance,
                context.Message.Language.DisplayName,
                context.Message.EventDate);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}

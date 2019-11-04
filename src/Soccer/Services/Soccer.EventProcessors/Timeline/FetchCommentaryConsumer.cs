using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Timelines.QueueMessages;
using Soccer.Database.Timelines.Commands;
using Soccer.EventProcessors.Leagues.Filters;

namespace Soccer.EventProcessors.Timeline
{
    public class FetchCommentaryConsumer : IConsumer<IMatchCommentaryFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IMajorLeagueFilter<string, bool> majorLeagueFilter;

        public FetchCommentaryConsumer(
            IDynamicRepository dynamicRepository,
            IMajorLeagueFilter<string, bool> majorLeagueFilter)
        {
            this.dynamicRepository = dynamicRepository;
            this.majorLeagueFilter = majorLeagueFilter;
        }

        public async Task Consume(ConsumeContext<IMatchCommentaryFetchedMessage> context)
        {
            //TODO validate major leagues

            if (context.Message == null
                || context.Message.Commentary == null
                || !(await majorLeagueFilter.Filter(context.Message.LeagueId)))
            {
                return;
            }

            await dynamicRepository.ExecuteAsync(new InsertCommentaryCommand(
                context.Message.MatchId,
                context.Message.Commentary.TimelineId,
                context.Message.Commentary.Commentaries,
                context.Message.Language));
        }
    }
}
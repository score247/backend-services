namespace Soccer.EventProcessors.Matches
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Events;
    using Soccer.Core.Matches.Models;
    using Soccer.Database.Matches.Commands;
    using Soccer.Database.Matches.Criteria;

    public class UpdateLiveMatchResultConsumer : IConsumer<ILiveMatchResultUpdatedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public UpdateLiveMatchResultConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<ILiveMatchResultUpdatedMessage> context)
        {
            var message = context.Message;

            // query live matches by region
            //var liveMatches = await dynamicRepository.FetchAsync<Match>(new GetLiveMatchesCriteria(language, dateTimeNowFunc().AddDays(-1).DateTime));

            // if exist in db but not in message -> remove and send decrease

            // if not exist in db but in message -> insert and send increase

            //var command = new UpdateLiveMatchResultCommand(message.MatchId, message.MatchResult);

            //await dynamicRepository.ExecuteAsync(command);
        }
    }
}
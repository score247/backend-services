namespace Soccer.EventProcessors.Matches
{
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Database.Matches.Commands;
    using System.Threading.Tasks;

    public class FetchPostMatchesConsumer : IConsumer<IPostMatchUpdatedResultMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchPostMatchesConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IPostMatchUpdatedResultMessage> context)
        {
            var message = context.Message;

            await UpdateMatchResultCommand(message.MatchId, message.Language, message.Result);
            await RemoveLiveMatch(message.MatchId);          
        }

        private async Task UpdateMatchResultCommand(string matchId, string language, MatchResult result)
        {   
            var command = new UpdateMatchResultCommand(matchId, language, result);

            await dynamicRepository.ExecuteAsync(command);
        }

        private async Task RemoveLiveMatch(string matchId)
        {
            var command = new RemoveLiveMatchCommand(matchId);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}
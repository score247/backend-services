namespace Soccer.EventProcessors.Odds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Odds.Messages;
    using Soccer.Core.Odds.Models;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Database.Matches.Criteria;
    using Soccer.Database.Odds.Commands;
    using Soccer.Database.Odds.Criteria;

    public class OddsChangeConsumer : IConsumer<IOddsChangeMessage>
    {
        private const int maxGetMatchDate = 10;
        private readonly IDynamicRepository dynamicRepository;
        private readonly Func<DateTime> getCurrentTimeFunc;

        public OddsChangeConsumer(
            IDynamicRepository dynamicRepository,
            Func<DateTime> getCurrentTimeFunc)
        {
            this.dynamicRepository = dynamicRepository;
            this.getCurrentTimeFunc = getCurrentTimeFunc;
        }

        public async Task Consume(ConsumeContext<IOddsChangeMessage> context)
        {
            var message = context.Message;
            // TODO: Wait for Get Match API
            var availableMatches = await GetMatch();
            foreach (var matchOdds in message.MatchOddsList)
            {
                if (availableMatches.Any(match => match.Id == matchOdds.MatchId))
                {
                    await InsertOdds(matchOdds, message.IsForceInsert);
                }
            }
        }

        private async Task<bool> InsertOdds(MatchOdds matchOdds, bool forceInsert)
        {
            var insertOddsList = new List<BetTypeOdds>();

            foreach (var betTypeOdds in matchOdds.BetTypeOddsList)
            {
                var lastOddsList = await GetOddsData(matchOdds.MatchId, betTypeOdds.Id);
                var lastOdds = lastOddsList.FirstOrDefault(odds => odds.Bookmaker.Id == betTypeOdds.Bookmaker.Id);

                if (lastOdds == null
                    || forceInsert
                    || (lastOdds.LastUpdatedTime < matchOdds.LastUpdated
                        && lastOdds.Equals(betTypeOdds)))
                {
                    betTypeOdds.SetLastUpdatedTime(matchOdds.LastUpdated ?? DateTime.Now);
                    insertOddsList.Add(betTypeOdds);
                }
            }

            if (insertOddsList.Any())
            {
                return await InsertOdds(insertOddsList, matchOdds.MatchId);
            }

            return true;
        }

        private async Task<IEnumerable<Match>> GetMatch()
        {
            // TODO: should use cache to cache match list here
            var utcTime = getCurrentTimeFunc().ToUniversalTime();
            var matches = await dynamicRepository.FetchAsync<Match>(
                new GetMatchesByDateRangeCriteria(
                    utcTime.Date,
                    utcTime.AddDays(maxGetMatchDate),
                    Language.en_US));

            return matches;
        }

        private async Task<IEnumerable<BetTypeOdds>> GetOddsData(string matchId, int betTypeId)
            => await dynamicRepository.FetchAsync<BetTypeOdds>(new GetOddsCriteria(matchId, betTypeId));

        private async Task<bool> InsertOdds(
            IEnumerable<BetTypeOdds> betTypeOdds,
            string matchId)
            => (await dynamicRepository.ExecuteAsync(new InsertOddsCommand(betTypeOdds, matchId))) > 0;
    }
}
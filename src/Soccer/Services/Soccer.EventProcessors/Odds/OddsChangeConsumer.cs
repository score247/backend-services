namespace Soccer.EventProcessors.Odds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Odds;
    using Soccer.Core.Odds.Messages;
    using Soccer.Core.Odds.Models;
    using Soccer.Core.Odds.SignalREvents;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Database.Matches.Criteria;
    using Soccer.Database.Odds.Commands;
    using Soccer.Database.Odds.Criteria;

    public class OddsChangeConsumer : IConsumer<IOddsChangeMessage>
    {
        private const int maxGetMatchDate = 10;
        private readonly IBus messageBus;
        private readonly IDynamicRepository dynamicRepository;
        private readonly Func<DateTime> getCurrentTimeFunc;

        public OddsChangeConsumer(
            IDynamicRepository dynamicRepository,
            Func<DateTime> getCurrentTimeFunc,
            IBus messageBus)
        {
            this.dynamicRepository = dynamicRepository;
            this.getCurrentTimeFunc = getCurrentTimeFunc;
            this.messageBus = messageBus;
        }

        public async Task Consume(ConsumeContext<IOddsChangeMessage> context)
        {
            var message = context.Message;
            var availableMatches = await GetMatch();
            foreach (var matchOdds in message.MatchOddsList)
            {
                var availableMatch = availableMatches.FirstOrDefault(match => match.Id == matchOdds.MatchId);
                if (availableMatch != null)
                {
                    await InsertOdds(matchOdds, message.IsForceInsert, availableMatch);
                }
            }
        }

        private async Task InsertOdds(MatchOdds matchOdds, bool forceInsert, Match match)
        {
            var insertOddsList = new List<BetTypeOdds>();

            if (forceInsert
                && (matchOdds.BetTypeOddsList == null || !matchOdds.BetTypeOddsList.Any()))
            {
                await HandleMatchOddsMissingOddsInformation(matchOdds, insertOddsList);
            }
            else
            {
                await HandleMatchOdds(matchOdds, forceInsert, insertOddsList);
            }

            if (insertOddsList.Any())
            {
                await Task.WhenAll(
                    InsertOdds(insertOddsList, matchOdds.MatchId),
                    PushOddsEvent(matchOdds, insertOddsList, match));
            }
        }

        private async Task PushOddsEvent(MatchOdds matchOdds, IEnumerable<BetTypeOdds> betTypeOddsList, Match match)
        {
            match.TimeLines = await dynamicRepository.FetchAsync<TimelineEvent>(new GetTimelineEventsCriteria(matchOdds.MatchId));

            var oddsEvent = new List<OddsEvent>();

            foreach (var betTypeOdds in betTypeOddsList)
            {
                var oddsByBookmaker = await GetBookmakerOddsListByBetType(matchOdds.MatchId, betTypeOdds.Id, betTypeOdds.Bookmaker.Id);

                var oddsMovement = OddsMovementProcessor.BuildOddsMovements(match, oddsByBookmaker).FirstOrDefault();

                if(oddsMovement != null)
                {
                    oddsEvent.Add(new OddsEvent(betTypeOdds.Id, betTypeOdds.Bookmaker, oddsMovement));
                }
            }

            if(oddsEvent.Any())
            {
                await messageBus.Publish<IMatchEventOddsMessage>(new MatchEventOddsMessage(matchOdds.MatchId, oddsEvent));
            }
        }

        private async Task<List<BetTypeOdds>> GetBookmakerOddsListByBetType(string matchId, int betTypeId, string bookmakerId)
            => (await dynamicRepository.FetchAsync<BetTypeOdds>(new GetOddsCriteria(matchId, betTypeId, bookmakerId)))
                .OrderBy(bto => bto.LastUpdatedTime)
                .ToList();

        private async Task HandleMatchOdds(MatchOdds matchOdds, bool forceInsert, List<BetTypeOdds> insertOddsList)
        {
            foreach (var betTypeOdds in matchOdds.BetTypeOddsList)
            {
                var lastOddsList = await GetOddsData(matchOdds.MatchId, betTypeOdds.Id);
                var lastOdds = lastOddsList.FirstOrDefault(odds => odds.Bookmaker.Id == betTypeOdds.Bookmaker.Id);

                if (lastOdds == null
                    || forceInsert
                    || (lastOdds.LastUpdatedTime < matchOdds.LastUpdated
                        && !lastOdds.Equals(betTypeOdds)))
                {
                    betTypeOdds.SetLastUpdatedTime(matchOdds.LastUpdated ?? DateTime.Now);
                    insertOddsList.Add(betTypeOdds);
                }
            }
        }

        private async Task HandleMatchOddsMissingOddsInformation(MatchOdds matchOdds, List<BetTypeOdds> insertOddsList)
        {
            var lastOddsList = await GetOddsData(matchOdds.MatchId);
            var groupByBookmakers = lastOddsList.GroupBy(bto => new { bookmakerId = bto.Bookmaker.Id, bto.Id });

            foreach (var groupByBookmaker in groupByBookmakers)
            {
                var lastOddsByBookmaker = groupByBookmaker.OrderBy(bto => bto.LastUpdatedTime).FirstOrDefault();
                if (lastOddsByBookmaker != null)
                {
                    lastOddsByBookmaker.SetLastUpdatedTime(matchOdds.LastUpdated ?? DateTime.Now);
                    insertOddsList.Add(lastOddsByBookmaker);
                }
            }
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

        private async Task<IEnumerable<BetTypeOdds>> GetOddsData(string matchId, int betTypeId = 0)
            => (await dynamicRepository.FetchAsync<BetTypeOdds>(new GetOddsCriteria(matchId, betTypeId))).OrderByDescending(bto => bto.LastUpdatedTime);

        private async Task InsertOdds(
            IEnumerable<BetTypeOdds> betTypeOdds,
            string matchId)
        {
            await dynamicRepository.ExecuteAsync(new InsertOddsCommand(betTypeOdds, matchId));
        }
    }
}
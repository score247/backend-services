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
                    await InsertOdds(matchOdds, availableMatch, message.MatchEvent);
                }
            }
        }

        private async Task InsertOdds(MatchOdds matchOdds, Match match, MatchEvent matchEvent)
        {
            var isForceInsert = matchEvent != null;
            var oddsList = isForceInsert && matchOdds.IsBetTypeOddsListEmpty() 
                ? await BuildMatchOddsWithoutCurrentOddsInformation(matchOdds) 
                : await BuildMatchOdds(matchOdds, isForceInsert);

            if (oddsList.Any())
            {
                await Task.WhenAll(
                    InsertOdds(oddsList, matchOdds.MatchId),
                    PushOddsMovementEvent(match, oddsList, matchEvent),
                    PushOddsComparisonEvent(match, oddsList));
            }
        }


        private async Task PushOddsComparisonEvent(
            Match match,
            IEnumerable<BetTypeOdds> betTypeOddsList)
        {
            var processedBetTypeOddsList = new List<BetTypeOdds>();
            var betTypeOddsListGroups = betTypeOddsList.GroupBy(bto => bto.Id);
            foreach (var betTypeOddsListGroup in betTypeOddsListGroups)
            {
                var oddsByBookmakers = (await GetOddsData(match.Id, betTypeOddsListGroup.Key)).ToList();

                if (oddsByBookmakers.Any())
                {
                    foreach (var betTypeOddsBookmaker in betTypeOddsListGroup)
                    {
                        var openingOdds = oddsByBookmakers.LastOrDefault(o => o.Bookmaker?.Id == betTypeOddsBookmaker.Bookmaker?.Id);
                        if (openingOdds != null)
                        {
                            betTypeOddsBookmaker.AssignOpeningData(openingOdds.BetOptions);
                            processedBetTypeOddsList.Add(betTypeOddsBookmaker);
                        }
                    }
                }
            }

            var oddsComparisonMessage = new OddsComparisonMessage(match.Id, processedBetTypeOddsList);

            await messageBus.Publish<IOddsComparisonMessage>(oddsComparisonMessage);
        }

        private async Task PushOddsMovementEvent(
            Match match,
            IEnumerable<BetTypeOdds> betTypeOddsList,  
            MatchEvent matchEvent)
        {
            var oddsEvents = new List<OddsMovementEvent>();

            match.TimeLines = await GetMatchTimelines(match.Id, matchEvent);

            foreach (var betTypeOdds in betTypeOddsList)
            {
                var oddsByBookmaker = await GetBookmakerOddsListByBetType(match.Id, betTypeOdds.Id, betTypeOdds.Bookmaker.Id);

                var oddsMovement = OddsMovementProcessor
                    .BuildOddsMovements(match, oddsByBookmaker)
                    .FirstOrDefault();

                if (oddsMovement != null)
                {
                    oddsEvents.Add(new OddsMovementEvent(betTypeOdds.Id, betTypeOdds.Bookmaker, oddsMovement));
                }
            }

            if (oddsEvents.Any())
            {
                await messageBus.Publish<IOddsMovementMessage>(new OddsMovementMessage(match.Id, oddsEvents));
            }
        }

        private async Task<IList<TimelineEvent>> GetMatchTimelines(string matchId, MatchEvent matchEvent)
        {
            var timelines = (await dynamicRepository
                .FetchAsync<TimelineEvent>(new GetTimelineEventsCriteria(matchId)))
                .ToList();

            if (matchEvent != null 
                && !timelines.Any(tl => tl.Id == matchEvent.Timeline?.Id))
            {
                timelines.Add(matchEvent.Timeline);
            }

            return timelines;
        }

        private async Task<List<BetTypeOdds>> GetBookmakerOddsListByBetType(string matchId, int betTypeId, string bookmakerId)
            => (await dynamicRepository.FetchAsync<BetTypeOdds>(new GetOddsCriteria(matchId, betTypeId, bookmakerId)))
                .OrderBy(bto => bto.LastUpdatedTime)
                .ToList();

        private async Task<List<BetTypeOdds>> BuildMatchOdds(
            MatchOdds matchOdds, 
            bool forceInsert)
        {
            var insertOddsList = new List<BetTypeOdds>();

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

            return insertOddsList;
        }

        private async Task<List<BetTypeOdds>> BuildMatchOddsWithoutCurrentOddsInformation(MatchOdds matchOdds)
        {
            var insertOddsList = new List<BetTypeOdds>();
            var lastOddsList = await GetOddsData(matchOdds.MatchId);
            var groupByBookmakers = lastOddsList.GroupBy(bto 
                => new {
                    bookmakerId = bto.Bookmaker.Id,
                    bto.Id
                });

            foreach (var groupByBookmaker in groupByBookmakers)
            {
                var lastOddsByBookmaker = groupByBookmaker
                    .OrderBy(bto => bto.LastUpdatedTime)
                    .FirstOrDefault();

                if (lastOddsByBookmaker != null)
                {
                    lastOddsByBookmaker.SetLastUpdatedTime(matchOdds.LastUpdated ?? DateTime.Now);
                    insertOddsList.Add(lastOddsByBookmaker);
                }
            }

            return insertOddsList;
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
            => (await dynamicRepository
                .FetchAsync<BetTypeOdds>(new GetOddsCriteria(matchId, betTypeId)))
                .OrderByDescending(bto => bto.LastUpdatedTime);

        private async Task InsertOdds(
            IEnumerable<BetTypeOdds> betTypeOdds,
            string matchId)
        {
            await dynamicRepository.ExecuteAsync(new InsertOddsCommand(betTypeOdds, matchId));
        }
    }
}
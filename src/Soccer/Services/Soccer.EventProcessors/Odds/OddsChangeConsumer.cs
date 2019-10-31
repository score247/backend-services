namespace Soccer.EventProcessors.Odds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using MassTransit;
    using Newtonsoft.Json;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Odds;
    using Soccer.Core.Odds.Messages;
    using Soccer.Core.Odds.Models;
    using Soccer.Core.Odds.SignalREvents;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Database.Matches.Criteria;
    using Soccer.Database.Odds.Commands;
    using Soccer.Database.Odds.Criteria;
    using Soccer.EventProcessors.Shared.Configurations;

    public class OddsChangeConsumer : IConsumer<IOddsChangeMessage>
    {
        private const int twoItems = 2;
        private const int cacheMatchSeconds = 10;
        private const int maxGetMatchDate = 10;
        private readonly IBus messageBus;
        private readonly IDynamicRepository dynamicRepository;
        private readonly Func<DateTime> getCurrentTimeFunc;
        private readonly ICacheService cacheService;
        private readonly IAppSettings appSettings;
        private readonly ILogger logger;

        public OddsChangeConsumer(
            IDynamicRepository dynamicRepository,
            Func<DateTime> getCurrentTimeFunc,
            IBus messageBus,
            ICacheService cacheService,
            IAppSettings appSettings,
            ILogger logger)
        {
            this.dynamicRepository = dynamicRepository;
            this.getCurrentTimeFunc = getCurrentTimeFunc;
            this.messageBus = messageBus;
            this.cacheService = cacheService;
            this.appSettings = appSettings;
            this.logger = logger;
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
                    try
                    {
                        await ProcessOdds(matchOdds, availableMatch, message.MatchEvent);
                    }
                    catch (Exception ex)
                    {
                        await LogInformation(message, matchOdds, availableMatch, ex);
                    }
                }
            }
        }

        private async Task LogInformation(IOddsChangeMessage message, MatchOdds matchOdds, Match availableMatch, Exception ex)
        {
            var errorMessage = string.Join(
                                        "\r\n",
                                        ex.Message,
                                        $"MatchOdds: {JsonConvert.SerializeObject(matchOdds)}",
                                        $"Match: {JsonConvert.SerializeObject(availableMatch)}",
                                        $"MatchEvent: {JsonConvert.SerializeObject(message.MatchEvent)}");

            //TODO: temporary log info for future trace
            if (ex.Message.Contains("Value cannot be null", StringComparison.InvariantCultureIgnoreCase))
            {
                await logger.InfoAsync(errorMessage);
            }
            else
            {
                await logger.ErrorAsync(errorMessage, ex);
            }
        }

        private async Task ProcessOdds(MatchOdds matchOdds, Match match, MatchEvent matchEvent)
        {
            if (matchEvent != null
                && !OddsMovementProcessor.IsTimelineNeedMapWithOddsData(matchEvent.Timeline))
            {
                return;
            }

            var isForceInsert = matchEvent != null;
            var oddsList = isForceInsert && matchOdds.IsBetTypeOddsListEmpty()
                ? await BuildMatchOddsWithoutCurrentOddsInformation(matchOdds)
                : await BuildMatchOdds(matchOdds, match.EventDate.DateTime, isForceInsert);

            if (oddsList.Any())
            {
                await InsertOdds(oddsList.Select(o => o.Item2), matchOdds.MatchId);

                var publishOdds = oddsList.Where(o => o.Item1).Select(o => o.Item2);
                await Task.WhenAll(
                    PushOddsMovementEvent(match, publishOdds.ToList(), matchEvent),
                    PushOddsComparisonEvent(match, publishOdds.ToList()));
            }
        }

        private async Task PushOddsComparisonEvent(
            Match match,
            IEnumerable<BetTypeOdds> betTypeOddsList)
        {
            var betTypeOddsListGroups = betTypeOddsList.GroupBy(bto => bto.Id);

            foreach (var betTypeOddsListGroup in betTypeOddsListGroups)
            {
                var oddsByBookmakers = (await GetOddsDataAndFilterWithMinDate(match, betTypeOddsListGroup.Key)).ToList();

                foreach (var betTypeOddsBookmaker in betTypeOddsListGroup)
                {
                    var oddsByBookmaker = oddsByBookmakers.Where(o => o.Bookmaker?.Id == betTypeOddsBookmaker.Bookmaker?.Id);
                    var openingOdds = oddsByBookmaker.FirstOrDefault();

                    betTypeOddsBookmaker.AssignOpeningData(
                        openingOdds != null
                        ? openingOdds.BetOptions
                        : betTypeOddsBookmaker.BetOptions);

                    if (oddsByBookmaker.Count() >= twoItems)
                    {
                        var secondItem = oddsByBookmaker.ElementAt(oddsByBookmaker.Count() - twoItems);
                        OddsMovementProcessor.CalculateOddsTrend(betTypeOddsBookmaker.BetOptions, secondItem.BetOptions);
                    }
                }
            }

            var oddsComparisonMessage = new OddsComparisonMessage(match.Id, betTypeOddsList);

            await messageBus.Publish<IOddsComparisonMessage>(oddsComparisonMessage);
        }

        private async Task PushOddsMovementEvent(
            Match match,
            IEnumerable<BetTypeOdds> betTypeOddsList,
            MatchEvent matchEvent)
        {
            var oddsEvents = new List<OddsMovementEvent>();

            match.SetTimelines(await GetMatchTimelines(match.Id, matchEvent));

            foreach (var betTypeOdds in betTypeOddsList)
            {
                var oddsByBookmaker = await GetBookmakerOddsListByBetType(match.Id, betTypeOdds.Id, betTypeOdds.Bookmaker.Id);
                var oddsMovement = OddsMovementProcessor
                    .BuildOddsMovements(match, oddsByBookmaker, appSettings.NumOfDaysToShowOddsBeforeKickoffDate)
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

        private async Task<List<(bool, BetTypeOdds)>> BuildMatchOdds(
            MatchOdds matchOdds,
            DateTime eventDate,
            bool forceInsert)
        {
            var insertOddsList = new List<(bool, BetTypeOdds)>();
            var minDate = eventDate.AddDays(-appSettings.NumOfDaysToShowOddsBeforeKickoffDate).Date;

            foreach (var betTypeOdds in matchOdds.BetTypeOddsList)
            {
                var lastOddsList = await GetOddsData(matchOdds.MatchId, betTypeOdds.Id);
                var lastOdds = lastOddsList.LastOrDefault(odds => odds.Bookmaker.Id == betTypeOdds.Bookmaker.Id);

                if (lastOdds == null
                    || forceInsert
                    || (lastOdds.LastUpdatedTime < matchOdds.LastUpdated
                        && !lastOdds.Equals(betTypeOdds)))
                {
                    betTypeOdds.SetLastUpdatedTime(matchOdds.LastUpdated ?? DateTime.Now);
                    var isOddsNeedBePublished = lastOdds == null || matchOdds.LastUpdated >= minDate;

                    insertOddsList.Add((isOddsNeedBePublished, betTypeOdds));
                }
            }

            return insertOddsList;
        }

        private async Task<List<(bool, BetTypeOdds)>> BuildMatchOddsWithoutCurrentOddsInformation(MatchOdds matchOdds)
        {
            var insertOddsList = new List<(bool, BetTypeOdds)>();
            var lastOddsList = await GetOddsData(matchOdds.MatchId);
            var groupByBookmakers = lastOddsList.GroupBy(bto
                => new
                {
                    bookmakerId = bto.Bookmaker.Id,
                    bto.Id
                });

            foreach (var groupByBookmaker in groupByBookmakers)
            {
                var lastOddsByBookmaker = groupByBookmaker
                    .OrderBy(bto => bto.LastUpdatedTime)
                    .LastOrDefault();

                if (lastOddsByBookmaker != null)
                {
                    lastOddsByBookmaker.SetLastUpdatedTime(matchOdds.LastUpdated ?? DateTime.Now);
                    insertOddsList.Add((true, lastOddsByBookmaker));
                }
            }

            return insertOddsList;
        }

        private async Task<IEnumerable<Match>> GetMatch()
        {
            var utcTime = getCurrentTimeFunc().ToUniversalTime();
            var fromDate = utcTime.Date;
            var cacheKey = $"Odds_Match_{fromDate.ToString("ddMMyyyy")}";

            var matches = await cacheService.GetOrSetAsync(
                cacheKey,
                () => GetMatch(utcTime),
                new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(0, 0, cacheMatchSeconds)));

            return matches;
        }

        private IEnumerable<Match> GetMatch(DateTime utcTime)
            => dynamicRepository.Fetch<Match>(
                    new GetMatchesByDateRangeCriteria(
                        utcTime.Date,
                        utcTime.AddDays(maxGetMatchDate),
                        Language.en_US));

        private async Task<IEnumerable<BetTypeOdds>> GetOddsDataAndFilterWithMinDate(Match match, int betTypeId = 0)
        {
            var betTypeOddsList = await GetOddsData(match.Id, betTypeId);

            if (!betTypeOddsList.Any())
            {
                return betTypeOddsList;
            }

            var firstOdds = betTypeOddsList.FirstOrDefault();
            var minDate = match.EventDate.AddDays(-appSettings.NumOfDaysToShowOddsBeforeKickoffDate).Date;
            var filteredBetTypeOddsList = betTypeOddsList.Where(
                bto => bto.LastUpdatedTime == firstOdds.LastUpdatedTime
                        || bto.LastUpdatedTime >= minDate);

            return filteredBetTypeOddsList;
        }

        private async Task<IOrderedEnumerable<BetTypeOdds>> GetOddsData(string matchId, int betTypeId = 0)
            => (await dynamicRepository
                            .FetchAsync<BetTypeOdds>(new GetOddsCriteria(matchId, betTypeId)))
                            .OrderBy(bto => bto.LastUpdatedTime);

        private async Task InsertOdds(
            IEnumerable<BetTypeOdds> betTypeOdds,
            string matchId)
            => await dynamicRepository.ExecuteAsync(new InsertOddsCommand(betTypeOdds, matchId));
    }
}
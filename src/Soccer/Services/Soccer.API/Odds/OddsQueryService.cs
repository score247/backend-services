namespace Soccer.API.Odds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Soccer.API.Shared.Configurations;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Odds;
    using Soccer.Core.Odds.Models;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Database.Matches.Criteria;
    using Soccer.Database.Odds.Criteria;

    public interface IOddsQueryService
    {
        Task<MatchOdds> GetOdds(string matchId, int betTypeId, Language language);

        Task<MatchOddsMovement> GetOddsMovement(string matchId, int betTypeId, string bookmakerId, Language language);
    }

    public class OddsQueryService : IOddsQueryService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IAppSettings appSettings;

        public OddsQueryService(
            IDynamicRepository dynamicRepository,
            IAppSettings appSettings)
        {
            this.dynamicRepository = dynamicRepository;
            this.appSettings = appSettings;
        }

        public async Task<MatchOdds> GetOdds(string matchId, int betTypeId, Language language)
            => new MatchOdds(matchId, await GetBookmakerComparisonOdds(matchId, betTypeId));

        private async Task<IEnumerable<BetTypeOdds>> GetBookmakerComparisonOdds(string matchId, int betTypeId)
        {
            var oddsByBookmaker = (await GetOddsData(matchId, betTypeId)).GroupBy(o => o.Bookmaker?.Id);
            var match = await GetMatch(matchId, Language.en_US);
            var minDate = DateTime.MinValue;

            if (match != null)
            {
                minDate = match.EventDate.AddDays(-appSettings.NumOfDaysToShowOddsBeforeKickoffDate).Date;
            }

            var betTypeOdssList = oddsByBookmaker
                .Select(group => OddsMovementProcessor.AssignOpeningOddsToFirstOdds(group, minDate))
                .OrderBy(bto => bto.Bookmaker.Name);

            return betTypeOdssList;
        }

        private async Task<IEnumerable<BetTypeOdds>> GetOddsData(string matchId, int betTypeId)
            => await dynamicRepository.FetchAsync<BetTypeOdds>(new GetOddsCriteria(matchId, betTypeId));

        public async Task<MatchOddsMovement> GetOddsMovement(
            string matchId,
            int betTypeId,
            string bookmakerId,
            Language language)
        {
            var betTypeOddsList = await GetBookmakerOddsListByBetType(matchId, betTypeId, bookmakerId);
            var firstOdds = betTypeOddsList.FirstOrDefault();

            if (firstOdds == null)
            {
                return new MatchOddsMovement();
            }

            var match = await GetMatch(matchId, language);

            var oddsMovements = OddsMovementProcessor.BuildOddsMovements(match, betTypeOddsList, appSettings.NumOfDaysToShowOddsBeforeKickoffDate);

            return new MatchOddsMovement(matchId, firstOdds.Bookmaker, oddsMovements);
        }

        private async Task<List<BetTypeOdds>> GetBookmakerOddsListByBetType(string matchId, int betTypeId, string bookmakerId)
            => (await dynamicRepository
                .FetchAsync<BetTypeOdds>(new GetOddsCriteria(matchId, betTypeId, bookmakerId)))
                .OrderBy(bto => bto.LastUpdatedTime)
                .ToList();

        private async Task<Match> GetMatch(string id, Language language)
        {
            var match = await dynamicRepository.GetAsync<Match>(new GetMatchByIdCriteria(id, language));

            if (match != null)
            {
                var timelineEvents = await dynamicRepository.FetchAsync<TimelineEvent>(new GetTimelineEventsCriteria(id));

                if (timelineEvents?.Count() > 0)
                {
                    match.TimeLines = timelineEvents.OrderBy(t => t.Time);
                }

                return match;
            }

            return null;
        }
    }
}
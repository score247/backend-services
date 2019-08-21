namespace Soccer.API.Odds
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Soccer.API.Matches;
    using Soccer.API.Shared.Configurations;
    using Soccer.Core.Odds;
    using Soccer.Core.Odds.Models;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Database.Odds.Criteria;

    public interface IOddsQueryService
    {
        Task<MatchOdds> GetOdds(string matchId, int betTypeId, Language language);

        Task<MatchOddsMovement> GetOddsMovement(string matchId, int betTypeId, string bookmakerId, Language language);
    }

    public class OddsQueryService : IOddsQueryService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IMatchQueryService matchQueryService;
        private readonly IAppSettings appSettings;

        public OddsQueryService(
            IDynamicRepository dynamicRepository,
            IMatchQueryService matchQueryService,
            IAppSettings appSettings)
        {
            this.dynamicRepository = dynamicRepository;
            this.matchQueryService = matchQueryService;
            this.appSettings = appSettings;
        }

        public async Task<MatchOdds> GetOdds(string matchId, int betTypeId, Language language)
            => new MatchOdds(matchId, await GetBookmakerComparisonOdds(matchId, betTypeId));

        private async Task<IEnumerable<BetTypeOdds>> GetBookmakerComparisonOdds(string matchId, int betTypeId)
        {
            var oddsData = await GetOddsData(matchId, betTypeId);

            var betTypeOdssList = oddsData
                .GroupBy(o => o.Bookmaker?.Id)
                .Select(group => OddsMovementProcessor.AssignOpeningOddsToFirstOdds(group))
                .OrderBy(bto => bto.Bookmaker.Name);

            return betTypeOdssList;
        }

        private async Task<IEnumerable<BetTypeOdds>> GetOddsData(string matchId, int betTypeId)
        { 
            var oddsData = (await dynamicRepository.FetchAsync<BetTypeOdds>(new GetOddsCriteria(matchId, betTypeId)))
                            .OrderBy(o => o.LastUpdatedTime)
                            .AsEnumerable();
            var firstOdds = oddsData.FirstOrDefault();

            if (firstOdds != null)
            {
                var match = await matchQueryService.GetMatch(matchId, Language.en_US);
                if (match != null)
                {
                    var minDate = match.EventDate.AddDays(-appSettings.NumOfDaysToShowOddsBeforeKickoffDate).Date;

                    oddsData = oddsData.Where(
                        bto => bto.LastUpdatedTime == firstOdds.LastUpdatedTime
                                || bto.LastUpdatedTime >= minDate);
                }
            }

            return oddsData;
        }

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

            var match = await matchQueryService.GetMatch(matchId, language);

            var oddsMovements = OddsMovementProcessor.BuildOddsMovements(match, betTypeOddsList, appSettings.NumOfDaysToShowOddsBeforeKickoffDate);

            return new MatchOddsMovement(matchId, firstOdds.Bookmaker, oddsMovements);
        }

        private async Task<List<BetTypeOdds>> GetBookmakerOddsListByBetType(string matchId, int betTypeId, string bookmakerId)
            => (await dynamicRepository
                .FetchAsync<BetTypeOdds>(new GetOddsCriteria(matchId, betTypeId, bookmakerId)))
                .OrderBy(bto => bto.LastUpdatedTime)
                .ToList();
    }
}
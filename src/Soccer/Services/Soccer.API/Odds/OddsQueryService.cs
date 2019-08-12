﻿namespace Soccer.API.Odds
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Soccer.API.Matches;
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

        public OddsQueryService(
            IDynamicRepository dynamicRepository,
            IMatchQueryService matchQueryService)
        {
            this.dynamicRepository = dynamicRepository;
            this.matchQueryService = matchQueryService;
        }

        public async Task<MatchOdds> GetOdds(string matchId, int betTypeId, Language language)
            => new MatchOdds(matchId, await GetBookmakerComparisonOdds(matchId, betTypeId));

        private async Task<IEnumerable<BetTypeOdds>> GetBookmakerComparisonOdds(string matchId, int betTypeId)
        {
            var oddsByBookmaker = (await GetOddsData(matchId, betTypeId)).GroupBy(o => o.Bookmaker?.Id);

            var betTypeOdssList = oddsByBookmaker
                .Select(group => AssignOpeningOddsToFirstOdds(group))
                .OrderBy(bto => bto.Bookmaker.Name)
                .AsEnumerable();

            return betTypeOdssList;
        }

        private static BetTypeOdds AssignOpeningOddsToFirstOdds(IGrouping<string, BetTypeOdds> group)
        {
            var orderedGroup = group.OrderByDescending(bto => bto.LastUpdatedTime);
            var first = orderedGroup.First();
            var last = orderedGroup.Last();

            first.AssignOpeningData(last.BetOptions);

            return first;
        }

        private async Task<IEnumerable<BetTypeOdds>> GetOddsData(string matchId, int betTypeId)
            => await dynamicRepository.FetchAsync<BetTypeOdds>(new GetOddsCriteria(matchId, betTypeId));

        public async Task<MatchOddsMovement> GetOddsMovement(string matchId, int betTypeId, string bookmakerId, Language language)
        {
            var betTypeOddsList = await GetBookmakerOddsListByBetType(matchId, betTypeId, bookmakerId);
            var firstOdds = betTypeOddsList.FirstOrDefault();

            if (firstOdds == null)
            {
                return new MatchOddsMovement();
            }

            var match = await matchQueryService.GetMatch(matchId, language);

            var oddsMovements = OddsMovementProcessor.BuildOddsMovements(match, betTypeOddsList);

            return new MatchOddsMovement(matchId, firstOdds.Bookmaker, oddsMovements);
        }

        private async Task<List<BetTypeOdds>> GetBookmakerOddsListByBetType(string matchId, int betTypeId, string bookmakerId)
            => (await dynamicRepository.FetchAsync<BetTypeOdds>(new GetOddsCriteria(matchId, betTypeId, bookmakerId)))
                .OrderBy(bto => bto.LastUpdatedTime)
                .ToList();
    }
}
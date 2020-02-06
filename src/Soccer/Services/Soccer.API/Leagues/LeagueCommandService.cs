namespace Soccer.API.Leagues
{
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Database.Leagues.Commands;

    public interface ILeagueCommandService
    {
        Task<bool> UpdateLeagueAbbreviation();
    }

    public class LeagueCommandService : ILeagueCommandService
    {
        private readonly ILeagueQueryService leagueQueryService;
        private readonly IDynamicRepository dynamicRepository;

        public LeagueCommandService(
            ILeagueQueryService leagueQueryService,
            IDynamicRepository dynamicRepository)
        {
            this.leagueQueryService = leagueQueryService;
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<bool> UpdateLeagueAbbreviation()
        {
            var leagues = await leagueQueryService.GetMajorLeagues(Language.en_US);

            if (leagues == null || !leagues.Any())
            {
                return false;
            }

            foreach (var league in leagues)
            {
                if (!string.IsNullOrWhiteSpace(league?.Name))
                {
                    var leagueAbbreviation = string.Join(string.Empty, 
                                                        league.Name.Split(' ', System.StringSplitOptions.RemoveEmptyEntries)
                                                        .Select(word => string.IsNullOrWhiteSpace(word)
                                                                            ? string.Empty
                                                                            : word[0].ToString().ToUpperInvariant()));

                    await dynamicRepository.ExecuteAsync(new UpdateLeagueAbbreviationCommand(league.Id, leagueAbbreviation));
                }
            }

            return true;
        }
    }
}
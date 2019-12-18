using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Leagues.Extensions
{
    public static class LeagueRoundExtension
    {
        public static bool HasGroupStage(this LeagueRound leagueRound)
        => leagueRound?.Type != LeagueRoundType.GroupRound || !string.IsNullOrWhiteSpace(leagueRound?.Group);
    }
}

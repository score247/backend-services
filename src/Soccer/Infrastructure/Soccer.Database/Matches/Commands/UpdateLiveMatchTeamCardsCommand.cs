namespace Soccer.Database.Matches.Commands
{
    using System;
    using Score247.Shared.Enumerations;
    using Soccer.Database._Shared.Extensions;

    public class UpdateLiveMatchTeamCardsCommand : BaseCommand
    {
        public UpdateLiveMatchTeamCardsCommand(
            string matchId,
            bool isHome,
            int redCards,
            int yellowRedCards,
            int yellowCards,
            DateTimeOffset eventDate = default)
            : base(eventDate)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            TeamIndex = isHome ? 0 : 1;
            RedCards = redCards;
            YellowRedCards = yellowRedCards;
            YellowCards = yellowCards;
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public int TeamIndex { get; }

        public int RedCards { get; }

        public int YellowRedCards { get; }

        public int YellowCards { get; }

        public override string GetSettingKey()
            => "LiveMatch_UpdateTeamCards".GetCorrespondingKey(EventDate, DateTimeOffset.Now);

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(MatchId);
    }
}
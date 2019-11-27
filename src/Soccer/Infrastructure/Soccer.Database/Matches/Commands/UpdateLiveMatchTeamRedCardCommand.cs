namespace Soccer.Database.Matches.Commands
{
    using System;
    using Score247.Shared.Enumerations;
    using Soccer.Database._Shared.Extensions;

    public class UpdateLiveMatchTeamRedCardCommand : BaseCommand
    {
        public UpdateLiveMatchTeamRedCardCommand(
            string matchId, 
            bool isHome, 
            int redCards, 
            int yellowRedCards,
            DateTimeOffset eventDate = default)
            : base(eventDate)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            TeamIndex = isHome ? 0 : 1;
            RedCards = redCards;
            YellowRedCards = yellowRedCards;
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public int TeamIndex { get; }

        public int RedCards { get; }

        public int YellowRedCards { get; }

        public override string GetSettingKey()
            => "LiveMatch_UpdateTeamRedCards".GetCorrespondingKey(EventDate);

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(MatchId);
    }
}
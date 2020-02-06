namespace Soccer.Database.Leagues.Commands
{
    public class UpdateLeagueAbbreviationCommand : BaseCommand
    {
        public UpdateLeagueAbbreviationCommand(
            string leagueId,
            string abbreviation)
        {
            LeagueId = leagueId;
            Abbreviation = abbreviation;
        }

        public string LeagueId { get; }

        public string Abbreviation { get; }

        public override string GetSettingKey()
            => "League_UpdateAbbreviation";


        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(LeagueId)
                && !string.IsNullOrWhiteSpace(Abbreviation);
    }
}
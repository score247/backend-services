namespace Soccer.Database.Matches.Commands
{
    using Score247.Shared.Enumerations;

    public class UpdateMatchRefereeAndAttendanceCommand : BaseCommand
    {
        public UpdateMatchRefereeAndAttendanceCommand(string matchId, string referee, int attendance, string language)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            Referee = referee;
            Attendance = attendance;
            Language = language;
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public string Referee { get; }

        public int Attendance { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Match_UpdateRefereeAndAttendance";

        public override bool IsValid() =>
            !string.IsNullOrWhiteSpace(MatchId)
            && !string.IsNullOrWhiteSpace(Language)
            && IsValidRefereeOrAttendance();

        public bool IsValidRefereeOrAttendance() => !string.IsNullOrWhiteSpace(Referee) || Attendance > 0;
    }
}

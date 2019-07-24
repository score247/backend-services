namespace Soccer.Core.Domain.Matches.Repositories.DbModels
{
    using System;
    using Score247.Shared.Base;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Domain.Matches.Models;

    public class MatchEntity : BaseEntity
    {
        public MatchEntity(Match match, string language)
        {
            Id = match.Id;
            LeagueId = match.League.Id;
            Region = match.Region;
            SportId = int.Parse(Sport.Soccer.Value);
            EventDate = match.EventDate;
            Value = match;
            Language = language;
        }

        public Match Value { get; }

        public DateTimeOffset EventDate { get; }

        public string LeagueId { get; }

        public string Language { get; }

        public int SportId { get; }

        public string Region { get; }
    }
}
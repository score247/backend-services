namespace Soccer.Core.Domain.Matches.Repositories.Commands
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Newtonsoft.Json;
    using Soccer.Core.Domain.Matches.Repositories.DbModels;

    public class InsertOrUpdatePreMatchesCommand : NonQueryCommand
    {
        public InsertOrUpdatePreMatchesCommand(IEnumerable<MatchEntity> matches)
        {
            Matches = JsonConvert.SerializeObject(
                matches,
                new JsonSerializerSettings()
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });
        }

        public string Matches { get; set; }

        public override string GetSettingKey() => "Score247_InsertOrUpdatePreMatches";

        public override bool IsValid() => !string.IsNullOrEmpty(Matches);
    }
}
using System.Collections.Generic;
using Soccer.Core.Teams.Models;

namespace Soccer.Core.Teams.QueueMessages
{
    public interface ITeamsFetchedMessage
    {
        IList<TeamProfile> Teams { get; }

        string Language { get; }
    }

    public class TeamsFetchedMessage : ITeamsFetchedMessage
    {
        public TeamsFetchedMessage(IList<TeamProfile> teams, string language)
        {
            Teams = teams;
            Language = language;
        }

        public IList<TeamProfile> Teams { get; }

        public string Language { get; }
    }
}

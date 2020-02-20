using System.Collections.Generic;
using Soccer.Core.Teams.Models;

namespace Soccer.Core.Teams.QueueMessages
{
    public interface ITeamsFetchedMessage
    {
        string Language { get; }

        IList<TeamProfile> Teams { get; }
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

using System.Collections.Generic;

namespace Soccer.Core.Leagues.Models
{
    public class Note
    {
        public GroupLog Group { get; }

        public Note(GroupLog groupNote)
        {
            Group = groupNote;
        }
    }
}
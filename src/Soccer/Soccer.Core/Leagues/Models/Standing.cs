using System;
using System.Collections.Generic;
using System.Text;

namespace Soccer.Core.Leagues.Models
{
    public class Standing
    {
        public string TieBreakRule { get; }
        public string Type { get; }
        public IEnumerable<GroupStanding> Groups { get; set; }

        public Standing(
            string tieBreakRule,
            string type,
            IEnumerable<GroupStanding> groups)
        {
            TieBreakRule = tieBreakRule;
            Type = type;
            Groups = groups;
        }
    }
}
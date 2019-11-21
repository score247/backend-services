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
    }
}
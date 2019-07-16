using System;
using System.Collections.Generic;
using System.Text;

namespace Soccer.Core.Domain.Matches.Models
{
    using Soccer.Core.Enumerations;

    public class MatchPeriod
    {
        public int HomeScore { get; set; }

        public int AwayScore { get; set; }

        public PeriodType PeriodType { get; set; }

        public int Number { get; set; }
    }
}
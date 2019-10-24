﻿using System;
using System.Collections.Generic;
using System.Text;
using Soccer.Core.Matches.Models;
using Soccer.Core.Timeline.Models;

namespace Soccer.DataProviders.SportRadar.Matches.Models
{
    public class MatchTimeline
    {
        public MatchTimeline(Match match, IEnumerable<TimelineCommentary> timelineCommentaries)
        {
            Match = match;
            TimelineCommentaries = timelineCommentaries;
        }

        public Match Match { get; }

        public IEnumerable<TimelineCommentary> TimelineCommentaries { get; }
    }
}

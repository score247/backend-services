﻿namespace Soccer.Core.Teams.Models
{
    using System.Collections.Generic;
    using Score247.Shared.Base;

    public class Team : BaseModel
    {
        public string Country { get; set; }

        public string CountryCode { get; set; }

        public string Flag { get; set; }

        public bool IsHome { get; set; }

        public IEnumerable<Player> Players { get; set; }

        public TeamStatistic Statistic { get; set; }

        public Coach Coach { get; set; }

        public string Formation { get; set; }

        public string Abbreviation { get; set; }

        public IEnumerable<Player> Substitutions { get; set; }
    }
}
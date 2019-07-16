using System;
using System.Collections.Generic;
using System.Text;

namespace Soccer.Core.Domain.Matches.Models
{
    using Soccer.Core.Base;

    public class Venue : BaseEntity
    {
        public int Capacity { get; set; }

        public string CityName { get; set; }

        public string CountryName { get; set; }

        public string CountryCode { get; set; }
    }
}
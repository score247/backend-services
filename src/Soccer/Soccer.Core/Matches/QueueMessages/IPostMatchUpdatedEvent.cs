﻿namespace Soccer.Core.Matches.Events
{
    using System.Collections.Generic;
    using Soccer.Core.Matches.Models;

    public interface IPostMatchUpdatedEvent
    {
        IList<Match> Matches { get; }

        string Language { get; }
    }
}
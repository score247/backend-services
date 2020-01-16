﻿using System.Collections.Generic;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Matches.QueueMessages
{
    public interface ILiveMatchFetchedMessage
    {
        Language Language { get; }

        IEnumerable<Match> Matches { get; }

        IReadOnlyList<string> Regions { get; }
    }

    public class LiveMatchFetchedMessage : ILiveMatchFetchedMessage
    {
        public LiveMatchFetchedMessage(Language language, IEnumerable<Match> matches, IReadOnlyList<string> regions)
        {
            Language = language;
            Matches = matches;
            Regions = regions;
        }

        public Language Language { get; }

        public IEnumerable<Match> Matches { get; }

        public IReadOnlyList<string> Regions { get; }
    }
}
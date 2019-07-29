namespace Soccer.DataProviders.SportRadar.Matches.DataMappers
{
    using System.Collections.Generic;
    using System.Linq;
    using Score247.Shared.Enumerations;
    using Soccer.Core._Shared.Enumerations;
    using Soccer.Core.Matches.Models;
    using Soccer.DataProviders.SportRadar.Leagues.DataMappers;
    using Soccer.DataProviders.SportRadar.Matches.Dtos;
    using Soccer.DataProviders.SportRadar.Teams.DataMappers;

    public static class MatchMapper
    {
        public static Match MapMatch(SportEventDto sportEvent, SportEventStatusDto sportEventStatus, string region)
        {
            var match = new Match
            {
                Id = sportEvent.id,
                EventDate = sportEvent.scheduled,
                Teams = TeamMapper.MapTeams(sportEvent),
                League = LeagueMapper.MapLeague(sportEvent.tournament),
                LeagueRound = LeagueMapper.MapLeagueRound(sportEvent.tournament_round),
                MatchResult = MapMatchResult(sportEventStatus),
                Region = region,
                Venue = MapVenue(sportEvent.venue)
            };

            return match;
        }

        public static MatchResult MapMatchResult(SportEventStatusDto sportEventStatus = null)
        {
            var matchResult = new MatchResult
            {
                EventStatus = MatchStatus.NotStarted,
                MatchStatus = MatchStatus.NotStarted,
                HomeScore = 0,
                AwayScore = 0,
                MatchTime = 0
            };

            if (sportEventStatus != null)
            {
                matchResult = new MatchResult
                {
                    EventStatus = Enumeration.FromDisplayName<MatchStatus>(sportEventStatus.status),
                    HomeScore = sportEventStatus.home_score,
                    AwayScore = sportEventStatus.away_score,
                    WinnerId = sportEventStatus.winner_id
                };

                if (!string.IsNullOrWhiteSpace(sportEventStatus.aggregate_winner_id))
                {
                    matchResult.AggregateWinnerId = sportEventStatus.aggregate_winner_id;
                    matchResult.AggregateHomeScore = sportEventStatus.aggregate_home_score;
                    matchResult.AggregateAwayScore = sportEventStatus.aggregate_away_score;
                }

                if (!string.IsNullOrWhiteSpace(sportEventStatus.match_status))
                {
                    matchResult.MatchStatus = Enumeration.FromDisplayName<MatchStatus>(sportEventStatus.match_status);
                }

                matchResult.Period = sportEventStatus.period;

                if (sportEventStatus.period_scores != null
                        && sportEventStatus.period_scores.Any())
                {
                    matchResult.MatchPeriods = sportEventStatus.period_scores.Select(
                        periodScore =>
                        new MatchPeriod
                        {
                            HomeScore = periodScore.home_score,
                            AwayScore = periodScore.away_score,
                            PeriodType = Enumeration.FromDisplayName<PeriodType>(periodScore.type),
                            Number = periodScore.number
                        });

                    matchResult.MatchTime = sportEventStatus.clock == null
                        ? 0
                        : TimelineMapper.ParseMatchClock(sportEventStatus.clock.match_time);
                }
            }

            return matchResult;
        }

        public static Venue MapVenue(VenueDto venueDto)
        {
            var venue = new Venue();

            if (venueDto != null)
            {
                venue.Name = venueDto.name;
                venue.Capacity = venueDto.capacity;
                venue.CityName = venueDto.city_name;
                venue.CountryName = venueDto.country_name;
                venue.CountryCode = venueDto.country_code;
            }

            return venue;
        }

        public static MatchEvent MapMatchEvent(MatchEventDto pushEventDto)
        {
            var matchEvent = new MatchEvent
            {
                MatchId = pushEventDto.metadata.sport_event_id,
                MatchResult = MapMatchResult(pushEventDto.payload.sport_event_status),
                Timeline = TimelineMapper.MapTimeline(pushEventDto.payload.timeline)
            };

            return matchEvent;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.Extensions;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.SportRadar._Shared;
using Soccer.DataProviders.SportRadar.Leagues.DataMappers;
using Soccer.DataProviders.SportRadar.Matches.Dtos;
using Soccer.DataProviders.SportRadar.Teams.DataMappers;

namespace Soccer.DataProviders.SportRadar.Matches.DataMappers
{
    public static class MatchMapper
    {
        public static Match MapMatch(
            SportEventDto sportEvent,
            SportEventStatusDto sportEventStatus,
            SportEventConditions sportEventConditions,
            string region,
            Language language,
            IEnumerable<TimelineEvent> timelineEvents = null,
            Coverage coverage = null)
        {
            var league = LeagueMapper.MapLeague(sportEvent.tournament, region);
            var leagueRound = LeagueMapper.MapLeagueRound(sportEvent.tournament_round);
            var leagueGroupName = league.MapLeagueGroupName(leagueRound, language);

            return new Match(
                    sportEvent.id,
                    sportEvent.scheduled,
                    sportEvent.scheduled,
                    TeamMapper.MapTeams(sportEvent),
                    MapMatchResult(sportEvent.status, sportEventStatus),
                    league,
                    leagueRound,
                    timelineEvents,
                    null,
                    sportEventConditions?.attendance ?? 0,
                    MapVenue(sportEvent.venue),
                    PlayerNameConverter.Convert(sportEventConditions?.referee?.name, false),
                    region,
                    coverage,
                    LeagueMapper.MapLeagueSeason(sportEvent.season),
                    leagueGroupName);
        }

        public static MatchResult MapMatchResult(string status, SportEventStatusDto sportEventStatus = null)
        {
            var matchResult = new MatchResult(
                !string.IsNullOrWhiteSpace(status)
                    ? Enumeration.FromDisplayName<MatchStatus>(status) : MatchStatus.NotStarted,
                MatchStatus.NotStarted,
                0,
                new List<MatchPeriod>(),
                0,
                string.Empty,
                0,
                0,
                0,
                0,
                string.Empty);

            if (sportEventStatus != null)
            {
                matchResult = new MatchResult(
                    Enumeration.FromDisplayName<MatchStatus>(sportEventStatus.status),
                    !string.IsNullOrWhiteSpace(sportEventStatus.match_status)
                        ? Enumeration.FromDisplayName<MatchStatus>(sportEventStatus.match_status)
                        : null,
                    sportEventStatus.period,
                    sportEventStatus.period_scores?.Select(
                        periodScore =>
                            new MatchPeriod
                            {
                                HomeScore = periodScore.home_score,
                                AwayScore = periodScore.away_score,
                                PeriodType = Enumeration.FromDisplayName<PeriodType>(periodScore.type),
                                Number = periodScore.number
                            }),
                    sportEventStatus.clock == null
                        ? 0
                        : TimelineMapper.ParseMatchClock(sportEventStatus.clock.match_time),
                    sportEventStatus.winner_id,
                    sportEventStatus.home_score,
                    sportEventStatus.away_score,
                    sportEventStatus.aggregate_home_score,
                    sportEventStatus.aggregate_away_score,
                    sportEventStatus.aggregate_winner_id);
            }

            return matchResult;
        }

        public static Venue MapVenue(VenueDto venueDto)
            => (venueDto == null)
                ? null
                : new Venue(
                    venueDto.id,
                    venueDto.name,
                    venueDto.capacity,
                    venueDto.city_name,
                    venueDto.country_name,
                    venueDto.country_code);

        public static MatchEvent MapMatchEvent(string matchEventPayload)
        {
            var matchEventDto = JsonConvert.DeserializeObject<MatchEventDto>(matchEventPayload);

            if (matchEventDto.payload == null)
            {
                return default;
            }

            return new MatchEvent(
                    matchEventDto.metadata.tournament_id,
                    matchEventDto.metadata.sport_event_id,
                    MapMatchResult(string.Empty, matchEventDto.payload.sport_event_status),
                    TimelineMapper.MapTimeline(matchEventDto.payload.timeline));
        }
    }
}
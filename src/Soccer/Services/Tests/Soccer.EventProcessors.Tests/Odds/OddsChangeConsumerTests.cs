namespace Soccer.EventProcessors.Tests.Odds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using MassTransit;
    using NSubstitute;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Odds.Models;
    using Soccer.Database.Odds.Criteria;
    using Soccer.EventProcessors.Odds;
    using Soccer.EventProcessors.Shared.Configurations;
    using Xunit;

    [Trait("Soccer.EventProcessors", "Odds")]
    public class OddsChangeConsumerTests
    {
        private readonly IBus messageBus;
        private readonly IDynamicRepository dynamicRepository;
        private readonly Func<DateTime> getCurrentTimeFunc;
        private readonly ICacheService cacheService;
        private readonly OddsChangeConsumer oddsChangeConsumer;
        private readonly IAppSettings appSettings;
        private readonly ILogger logger;

        public OddsChangeConsumerTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            cacheService = Substitute.For<ICacheService>();
            messageBus = Substitute.For<IBus>();
            getCurrentTimeFunc = Substitute.For<Func<DateTime>>();
            appSettings = Substitute.For<IAppSettings>();
            logger = Substitute.For<ILogger>();
            oddsChangeConsumer = new OddsChangeConsumer(
                dynamicRepository,
                getCurrentTimeFunc,
                messageBus,
                cacheService,
                appSettings,
                logger);
        }

        [Fact]
#pragma warning disable S2699 // Tests should include assertions
        public async Task Consume_NoMatchExist_DoesNotExecuteGetOdds()
#pragma warning restore S2699 // Tests should include assertions
        {
            cacheService.GetOrSetAsync(
                Arg.Any<string>(),
                Arg.Any<Func<IEnumerable<Match>>>(),
                Arg.Any<CacheItemOptions>()).Returns(Enumerable.Empty<Match>());

            await dynamicRepository.DidNotReceive().FetchAsync<BetTypeOdds>(Arg.Any<GetOddsCriteria>());
        }

        ////[Fact]
        ////public void CheckIssue()
        ////{
        ////    var matchOddsJson = @"{'MatchId':'sr: match: 17380329','LastUpdated':'2019 - 09 - 03T23: 26:19Z','BetTypeOddsList':[{'Id':1,'Name':'1x2','Bookmaker':{'Id':'sr: book: 74','Name':'Bet365'},'LastUpdatedTime':'2019 - 09 - 03T23: 26:19Z','BetOptions':[{'Type':'home','LiveOdds':1.12,'OpeningOdds':1.12,'OptionValue':'','OpeningOptionValue':'','OddsTrend':null},{'Type':'away','LiveOdds':10.00,'OpeningOdds':10.00,'OptionValue':'','OpeningOptionValue':'','OddsTrend':null},{'Type':'draw','LiveOdds':8.00,'OpeningOdds':8.00,'OptionValue':'','OpeningOptionValue':'','OddsTrend':null}]},{'Id':2,'Name':'OverUnder','Bookmaker':{'Id':'sr: book: 74','Name':'Bet365'},'LastUpdatedTime':'2019 - 09 - 03T23: 26:19Z','BetOptions':[{'Type':'over','LiveOdds':1.44,'OpeningOdds':1.44,'OptionValue':'2.5','OpeningOptionValue':'2.5','OddsTrend':null},{'Type':'under','LiveOdds':2.70,'OpeningOdds':2.70,'OptionValue':'2.5','OpeningOptionValue':'2.5','OddsTrend':null}]}]}";
        ////    var matchJson = "{'EventDate':'2019-09-05T15:00:00+00:00','CurrentPeriodStartTime':'0001-01-01T00:00:00+00:00','Teams':[{'Country':'Georgia','CountryCode':'GEO','Flag':null,'IsHome':true,'Players':null,'Statistic':{'Possession':0,'FreeKicks':0,'ThrowIns':0,'GoalKicks':0,'ShotsBlocked':0,'ShotsOnTarget':0,'ShotsOffTarget':0,'CornerKicks':0,'Fouls':0,'ShotsSaved':0,'Offsides':0,'YellowCards':0,'Injuries':0,'RedCards':0,'YellowRedCards':0},'Coach':null,'Formation':null,'Abbreviation':'GEO','Substitutions':null,'Id':'sr:competitor:6373','Name':'Georgia'},{'Country':'Liechtenstein','CountryCode':'LIE','Flag':null,'IsHome':false,'Players':null,'Statistic':{'Possession':0,'FreeKicks':0,'ThrowIns':0,'GoalKicks':0,'ShotsBlocked':0,'ShotsOnTarget':0,'ShotsOffTarget':0,'CornerKicks':0,'Fouls':0,'ShotsSaved':0,'Offsides':0,'YellowCards':0,'Injuries':0,'RedCards':0,'YellowRedCards':0},'Coach':null,'Formation':null,'Abbreviation':'LIE','Substitutions':null,'Id':'sr:competitor:8030','Name':'Liechtenstein'}],'MatchResult':{'MatchStatus':{'DisplayName':'not_started','Value':1},'EventStatus':{'DisplayName':'not_started','Value':1},'Period':0,'MatchPeriods':null,'MatchTime':0,'WinnerId':null,'HomeScore':0,'AwayScore':0,'AggregateHomeScore':0,'AggregateAwayScore':0,'AggregateWinnerId':null},'League':{'Order':0,'Flag':null,'Category':{'CountryCode':null,'Id':'sr:category:392','Name':'International Youth'},'Id':'sr:tournament:26','Name':'U21 Euro Qualification'},'LeagueRound':{'Type':{'DisplayName':'group','Value':2},'Name':null,'Number':1,'Phase':null,'Group':'2'},'TimeLines':[],'LatestTimeline':null,'Attendance':0,'Venue':{'Capacity':54549,'CityName':'Tbilisi','CountryName':'Georgia','CountryCode':'GEO','Id':null,'Name':'Boris Paichadze Dinamo Arena'},'Referee':null,'Region':'intl','Id':'sr:match:17380329','CreatedTime':'0001-01-01T00:00:00+00:00','ModifiedTime':'0001-01-01T00:00:00+00:00'}";

        ////    var matchOdds = JsonConvert.DeserializeObject<MatchOdds>(matchOddsJson);
        ////    var match = JsonConvert.DeserializeObject<Match>(matchJson);

        ////    var oddsMovement = OddsMovementProcessor.BuildOddsMovements(match, matchOdds.BetTypeOddsList.ToList(), 2);
        ////}
    }
}
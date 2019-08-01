SELECT * FROM `Match`;
SELECT * FROM `LiveMatch`;
TRUNCATE `Timeline`;
TRUNCATE  `Match`;
TRUNCATE  `LiveMatch`;
SELECT `Value` FROM score247_db2.`Match` WHERE Language LIKE 'en-US';
SELECT NonLiveMatch.`Value` 
    FROM score247_db2.Match as NonLiveMatch LEFT JOIN score247_db2.LiveMatch as LiveMatch
    ON NonLiveMatch.Id != LiveMatch.Id ;
SELECT `Value` FROM score247_db2.`Match` as Matches
    WHERE SportId = 1 AND  EventDate >  STR_TO_DATE('2019-07-22T14:00:00+00:00','%Y-%m-%dT%H:%i:%s')
     AND EventDate <  STR_TO_DATE('2019-07-26T14:00:00+00:00','%Y-%m-%dT%H:%i:%s')
    AND Language = 'en-US';
        
SELECT JSON_LENGTH('[{ "Value": "{ \"EventDate\": \"2019-07-22T15:00:00Z\" }" , "LeagueId":"sr:tournament:695", "Language":"en-US", "SportId":1, "Region":"intl", "Id":"sr:match:18116347", "CreatedTime":"0001-01-01T00:00:00+00:00", "ModifiedTime":"0001-01-01T00:00:00+00:00"}]');
SELECT JSON_EXTRACT('[ { "EventDate": "2019-07-22T15:00:00Z" }, { "EventDate": "2019-07-22T15:00:00Z" } ]', 
'$[0]'); 
 SELECT JSON_EXTRACT( '{"male" : 2000, "female" : 3000, "other" : 600}', '$');
select @js := JSON_OBJECT('new_time',"2019-07-24T14:00:00Z"  );

select CAST(JSON_UNQUOTE(JSON_EXTRACT(@js,'$.new_time')) as DATETIME);
 
 SELECT STR_TO_DATE('2019-07-22T14:00:00+00:00','%Y-%m-%dT%H:%i:%s') ;
SELECT * FROM `Timeline`;
 INSERT INTO `Timeline` VALUES (
			'sr:1234aa',
			JSON_UNQUOTE('{\"Type\":{\"IsMatchEnd\":false,\"IsPeriodStart\":false,\"IsScoreChange\":false,\"IsPenaltyShootout\":false,\"DisplayName\":\"match_started\",\"Value\":4},\"Time\":\"2019-06-20T10:00:32Z\",\"MatchTime\":0,\"Period\":0,\"PeriodType\":null,\"HomeScore\":0,\"AwayScore\":0,\"InjuryTimeAnnounced\":0,\"Description\":null,\"Outcome\":null,\"StoppageTime\":null,\"ShootoutHomeScore\":0,\"ShootoutAwayScore\":0,\"IsHomeShootoutScored\":false,\"IsAwayShootoutScored\":false,\"IsHome\":false,\"IsFirstShoot\":false,\"PenaltyStatus\":null,\"Id\":\"1\",\"CreatedTime\":\"0001-01-01T00:00:00+00:00\",\"ModifiedTime\":\"0001-01-01T00:00:00+00:00\"}'),
            now(),
            now());

CALL Score247_InsertTimeline('sr:1234aa', '{\"Type\":{\"IsMatchEnd\":false,\"IsPeriodStart\":false,\"IsScoreChange\":false,\"IsPenaltyShootout\":false,\"DisplayName\":\"match_started\",\"Value\":4},\"Time\":\"2019-06-20T10:00:32Z\",\"MatchTime\":0,\"Period\":0,\"PeriodType\":null,\"HomeScore\":0,\"AwayScore\":0,\"InjuryTimeAnnounced\":0,\"Description\":null,\"Outcome\":null,\"StoppageTime\":null,\"ShootoutHomeScore\":0,\"ShootoutAwayScore\":0,\"IsHomeShootoutScored\":false,\"IsAwayShootoutScored\":false,\"IsHome\":false,\"IsFirstShoot\":false,\"PenaltyStatus\":null,\"Id\":\"1\",\"CreatedTime\":\"0001-01-01T00:00:00+00:00\",\"ModifiedTime\":\"0001-01-01T00:00:00+00:00\"}');

	INSERT INTO `LiveMatch`
	 SELECT 
			Id, 
			JSON_REPLACE(`Value`,  '$.MatchResult', JSON_UNQUOTE(JSON_EXTRACT("{\"MatchStatus\":{\"DisplayName\":\"1st_half\",\"Value\":6},\"EventStatus\":{\"DisplayName\":\"live\",\"Value\":5},\"Period\":1,\"MatchPeriods\":[{\"HomeScore\":0,\"AwayScore\":0,\"PeriodType\":{\"IsPenalties\":false,\"DisplayName\":\"regular_period\",\"Value\":1},\"Number\":1}],\"MatchTime\":0,\"WinnerId\":null,\"HomeScore\":0,\"AwayScore\":0,\"AggregateHomeScore\":0,\"AggregateAwayScore\":0,\"AggregateWinnerId\":null}", '$'))) as Value,
			`Language`,
			SportId,
			LeagueId,
			EventDate,
			Region,
			now(),
			now()
			FROM `Match`
             WHERE `SportId` = 1 AND Id = 'sr:match:16542453'
	 ON DUPLICATE KEY UPDATE
		`Value` = JSON_REPLACE(VALUES(`Value`) ,  '$.MatchResult', JSON_UNQUOTE(JSON_EXTRACT("{\"MatchStatus\":{\"DisplayName\":\"1st_half\",\"Value\":6},\"EventStatus\":{\"DisplayName\":\"live\",\"Value\":5},\"Period\":1,\"MatchPeriods\":[{\"HomeScore\":0,\"AwayScore\":0,\"PeriodType\":{\"IsPenalties\":false,\"DisplayName\":\"regular_period\",\"Value\":1},\"Number\":1}],\"MatchTime\":0,\"WinnerId\":null,\"HomeScore\":0,\"AwayScore\":0,\"AggregateHomeScore\":0,\"AggregateAwayScore\":0,\"AggregateWinnerId\":null}", '$'))),
		ModifiedTime = now();
        SELECT * FROM `LiveMatch`;
        TRUNCATE  `LiveMatch`;
CALL Score247_UpdateLiveMatchResult(1, 'sr:match:16542453',  "{\"MatchStatus\":{\"DisplayName\":\"1st_half\",\"Value\":6},\"EventStatus\":{\"DisplayName\":\"live\",\"Value\":5},\"Period\":1,\"MatchPeriods\":[{\"HomeScore\":0,\"AwayScore\":0,\"PeriodType\":{\"IsPenalties\":false,\"DisplayName\":\"regular_period\",\"Value\":1},\"Number\":1}],\"MatchTime\":0,\"WinnerId\":null,\"HomeScore\":0,\"AwayScore\":0,\"AggregateHomeScore\":0,\"AggregateAwayScore\":0,\"AggregateWinnerId\":null}");

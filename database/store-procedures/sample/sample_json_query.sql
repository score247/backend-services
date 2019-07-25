SELECT * FROM score247_db2.`Match`;
TRUNCATE  score247_db2.`Match`;
TRUNCATE  score247_db2.`LiveMatch`;
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
 SELECT JSON_EXTRACT( '{"male" : 2000, "female" : 3000, "other" : 600}', '$.male');
select @js := JSON_OBJECT('new_time',"2019-07-24T14:00:00Z"  );

select CAST(JSON_UNQUOTE(JSON_EXTRACT(@js,'$.new_time')) as DATETIME);
 
 SELECT STR_TO_DATE('2019-07-22T14:00:00+00:00','%Y-%m-%dT%H:%i:%s') ;



CALL Score247_InsertOrUpdatePreMatches('[{"Value":{"EventDate":"2019-07-24T14:00:00Z","Teams":[{"Country":"Portugal","CountryCode":"PRT","Flag":null,"IsHome":true,"Players":null,"Statistic":null,"Coach":null,"Formation":null,"Abbreviation":"POR","Substitutions":null,"Id":"sr:competitor:22549","Name":"Portugal"},{"Country":"Ireland","CountryCode":"IRL","Flag":null,"IsHome":false,"Players":null,"Statistic":null,"Coach":null,"Formation":null,"Abbreviation":"RIR","Substitutions":null,"Id":"sr:competitor:22551","Name":"Ireland"}],"MatchResult":{"MatchStatus":{"DisplayName":"NotStarted","Value":"not_started"},"EventStatus":{"DisplayName":"NotStarted","Value":"not_started"},"Period":0,"MatchPeriods":null,"MatchTime":0,"WinnerId":null,"HomeScore":0,"AwayScore":0,"AggregateHomeScore":0,"AggregateAwayScore":0,"AggregateWinnerId":null},"League":{"Order":0,"Flag":null,"Category":{"CountryCode":null,"Id":"sr:category:392","Name":"International Youth"},"Id":"sr:tournament:258","Name":"U19 European Ch.ship"},"LeagueRound":{"Type":{"DisplayName":"Cup","Value":"cup"},"Name":"semifinal","Number":0,"Phase":"playoffs","Group":null},"TimeLines":null,"LatestTimeline":null,"Attendance":0,"Venue":{"Capacity":0,"CityName":null,"CountryName":null,"CountryCode":null,"Id":null,"Name":null},"Referee":null,"Region":"intl","Id":"sr:match:18837582","Name":null},"EventDate":"2019-07-24T14:00:00+00:00","LeagueId":"sr:tournament:258","Language":"en-US","SportId":1,"Region":"intl","Id":"sr:match:18837582","CreatedTime":"0001-01-01T00:00:00+00:00","ModifiedTime":"0001-01-01T00:00:00+00:00"},{"Value":{"EventDate":"2019-07-24T17:00:00Z","Teams":[{"Country":"France","CountryCode":"FRA","Flag":null,"IsHome":true,"Players":null,"Statistic":null,"Coach":null,"Formation":null,"Abbreviation":"FRA","Substitutions":null,"Id":"sr:competitor:22509","Name":"France"},{"Country":"Spain","CountryCode":"ESP","Flag":null,"IsHome":false,"Players":null,"Statistic":null,"Coach":null,"Formation":null,"Abbreviation":"ESP","Substitutions":null,"Id":"sr:competitor:22560","Name":"Spain"}],"MatchResult":{"MatchStatus":{"DisplayName":"NotStarted","Value":"not_started"},"EventStatus":{"DisplayName":"NotStarted","Value":"not_started"},"Period":0,"MatchPeriods":null,"MatchTime":0,"WinnerId":null,"HomeScore":0,"AwayScore":0,"AggregateHomeScore":0,"AggregateAwayScore":0,"AggregateWinnerId":null},"League":{"Order":0,"Flag":null,"Category":{"CountryCode":null,"Id":"sr:category:392","Name":"International Youth"},"Id":"sr:tournament:258","Name":"U19 European Ch.ship"},"LeagueRound":{"Type":{"DisplayName":"Cup","Value":"cup"},"Name":"semifinal","Number":0,"Phase":"playoffs","Group":null},"TimeLines":null,"LatestTimeline":null,"Attendance":0,"Venue":{"Capacity":0,"CityName":null,"CountryName":null,"CountryCode":null,"Id":null,"Name":null},"Referee":null,"Region":"intl","Id":"sr:match:18837584","Name":null},"EventDate":"2019-07-24T17:00:00","LeagueId":"sr:tournament:258","Language":"en-US","SportId":1,"Region":"intl","Id":"sr:match:18837584","CreatedTime":"0001-01-01T00:00:00+00:00","ModifiedTime":"0001-01-01T00:00:00+00:00"}]')
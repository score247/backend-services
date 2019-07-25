CREATE DEFINER=`user`@`%` PROCEDURE `Score247_GetMatchesByDateRange`(in sportId int, in fromDate datetime, in toDate datetime, in language text)
BEGIN
	(SELECT NonLiveMatch.`Value` 
    FROM score247_db2.Match as NonLiveMatch LEFT JOIN score247_db2.LiveMatch as LiveMatch
    ON NonLiveMatch.Id != LiveMatch.Id 
    WHERE NonLiveMatch.SportId = sportId
		AND NonLiveMatch.EventDate >=  fromDate
		AND NonLiveMatch.EventDate <=  toDate
		AND NonLiveMatch.`Language` = language)
	UNION ALL 
	(SELECT Value FROM score247_db2.LiveMatch
     WHERE SportId = sportId
		AND  EventDate >=  fromDate
		AND EventDate <=  toDate
		AND `Language` = language);
END
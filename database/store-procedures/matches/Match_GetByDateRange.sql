CREATE DEFINER=`user`@`%` PROCEDURE `Match_GetByDateRange`(IN sportId INT, IN fromDate DATETIME, IN toDate DATETIME, IN language TEXT)
BEGIN
	CREATE TEMPORARY TABLE IF NOT EXISTS temp_match AS
    (SELECT NonLiveMatch.`Value`, NonLiveMatch.EventDate, NonLiveMatch.LeagueId, NonLiveMatch.Id 
		FROM `Match` as NonLiveMatch 
		INNER JOIN `League` as League ON NonLiveMatch.LeagueId = League.Id
		WHERE NonLiveMatch.Id NOT IN
			(
			SELECT  Id
			FROM    LiveMatch as LM
			)
		   AND NonLiveMatch.SportId = sportId
			AND NonLiveMatch.EventDate >=  fromDate
			AND NonLiveMatch.EventDate <=  toDate
			AND NonLiveMatch.`Language` = language)
		UNION ALL 
		(SELECT Value, EventDate, LeagueId, Id FROM `LiveMatch` as LM
		 WHERE LM.SportId = sportId
			AND  LM.EventDate >=  fromDate
			AND LM.EventDate <=  toDate
			AND LM.`Language` = language);
    
    SELECT `Match`.Value AS Value, `Match`.EventDate AS EventDate, `Match`.LeagueId AS LeagueId, `Match`.Id AS Id
    FROM temp_match AS `Match`
	INNER JOIN `League` as League ON `Match`.LeagueId = League.Id
	ORDER BY League.Order, `Match`.EventDate, `Match`.LeagueId, `Match`.Id;

	DROP TEMPORARY TABLE temp_match;
END
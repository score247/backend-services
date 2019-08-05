CREATE DEFINER=`user`@`%` PROCEDURE `Match_GetByDateRange`(IN sportId INT, IN fromDate DATETIME, IN toDate DATETIME, IN language TEXT)
BEGIN
	(SELECT NonLiveMatch.`Value` 
    FROM `Match` as NonLiveMatch LEFT JOIN LiveMatch as LiveMatch
    ON NonLiveMatch.Id != LiveMatch.Id 
    WHERE NonLiveMatch.SportId = sportId
		AND NonLiveMatch.EventDate >=  fromDate
		AND NonLiveMatch.EventDate <=  toDate
		AND NonLiveMatch.`Language` = language)
	UNION ALL 
	(SELECT Value FROM `LiveMatch` as LM
     WHERE LM.SportId = sportId
		AND  LM.EventDate >=  fromDate
		AND LM.EventDate <=  toDate
		AND LM.`Language` = language);
END
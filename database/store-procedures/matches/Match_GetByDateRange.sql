CREATE DEFINER=`user`@`%` PROCEDURE `Match_GetByDateRange`(IN sportId INT, IN fromDate DATETIME, IN toDate DATETIME, IN language TEXT)
BEGIN
	(SELECT NonLiveMatch.`Value`, EventDate, Id 
    FROM `Match` as NonLiveMatch 
    WHERE Id NOT IN
		(
		SELECT  Id
		FROM    LiveMatch as LM
		)
       AND NonLiveMatch.SportId = sportId
		AND NonLiveMatch.EventDate >=  fromDate
		AND NonLiveMatch.EventDate <=  toDate
		AND NonLiveMatch.`Language` = language)
	UNION ALL 
	(SELECT Value, EventDate, Id FROM `LiveMatch` as LM
     WHERE LM.SportId = sportId
		AND  LM.EventDate >=  fromDate
		AND LM.EventDate <=  toDate
		AND LM.`Language` = language)
	ORDER BY EventDate, Id;
END
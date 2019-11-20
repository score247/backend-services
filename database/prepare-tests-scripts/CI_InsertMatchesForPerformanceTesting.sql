CREATE DEFINER=`root`@`%` PROCEDURE `CI_InsertMatchesForPerformanceTesting`(IN eventDate DATETIME, IN limitMatch INT)
BEGIN
	INSERT INTO `Match`
	SELECT 
		UUID(), 
		JSON_SET(`Value`,  '$.Id', UUID(), '$.EventDate', eventDate) as `Value`,
		`Language`,
		`SportId`,
		LeagueId,
		eventDate,
		Region,
		now(),
		now()
		FROM `Match`
        LIMIT limitMatch;
END
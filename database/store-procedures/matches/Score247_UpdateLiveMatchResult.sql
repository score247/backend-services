CREATE DEFINER=`user`@`%` PROCEDURE `Score247_UpdateLiveMatchResult`(
	IN sportId INT(11), 
    IN matchId VARCHAR(45), 
    IN matchResult TEXT)
BEGIN
	IF EXISTS (SELECT 1 FROM `Match` WHERE Id = matchId) THEN
    BEGIN
		 INSERT INTO `LiveMatch`
		 SELECT 
			Id, 
			JSON_REPLACE(`Value`,  '$.MatchResult', JSON_EXTRACT(matchResult, '$')) as `Value`,
			`Language`,
			`SportId`,
			LeagueId,
			EventDate,
			Region,
			now(),
			now()
			FROM `Match` as M
		 WHERE M.`SportId` = sportId AND M.Id = matchId
		 ON DUPLICATE KEY UPDATE
			`Value` = JSON_REPLACE(VALUES(`Value`),  '$.MatchResult', JSON_EXTRACT(matchResult, '$')),
			ModifiedTime = now();
    END;
    END IF;
END
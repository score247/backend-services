CREATE DEFINER=`user`@`%` PROCEDURE `Score247_UpdateMatchResult`(
	IN sportId INT, 
    IN matchId VARCHAR(45), 
    IN matchResult TEXT)
BEGIN
	UPDATE `Match` 
    SET Value =  JSON_REPLACE(VALUES(`Value`),  '$.MatchResult', JSON_EXTRACT(matchResult, '$'))
	WHERE SportId = sportId
		AND Id = matchId;
END
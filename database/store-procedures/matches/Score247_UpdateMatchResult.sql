CREATE DEFINER=`user`@`%` PROCEDURE `Score247_UpdateMatchResult`(
	IN sportId INT, 
    IN matchId VARCHAR(45), 
    IN matchResult TEXT, 
    IN language VARCHAR(10))
BEGIN
	UPDATE `Match` 
    SET Value = JSON_REPLACE(Value, '$.MatchResult', matchResult)
	WHERE SportId = sportId
		AND Id = matchId
        AND Language = language;
END
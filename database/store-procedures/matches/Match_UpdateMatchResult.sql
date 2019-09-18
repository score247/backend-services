CREATE DEFINER=`user`@`%` PROCEDURE `Match_UpdateMatchResult`(
	IN sportId INT, 
    IN matchId VARCHAR(45),     
    IN language VARCHAR(10),
    IN matchResult TEXT)
BEGIN
	UPDATE `Match` as M
    SET 
		Value = JSON_REPLACE(Value,  '$.MatchResult', JSON_EXTRACT(matchResult, '$')),
        ModifiedTime = now()
	WHERE M.SportId = sportId
		AND M.Id = matchId
        AND M.Language = language;
END
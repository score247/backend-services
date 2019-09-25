DROP procedure IF EXISTS `Match_UpdateCoverage`;

CREATE DEFINER=`user`@`%` PROCEDURE `Match_UpdateCoverage`(
	IN sportId INT, 
    IN matchId VARCHAR(45), 
    IN language VARCHAR(10),
    IN coverage TEXT)
BEGIN
	UPDATE `Match` as M
    SET 
		Value = JSON_SET(Value,  '$.Coverage', JSON_EXTRACT(coverage, '$')),
        ModifiedTime = now()
	WHERE M.SportId = sportId
		AND M.Id = matchId
        AND M.Language = language;
END
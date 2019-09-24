DROP procedure IF EXISTS `Match_UpdateMatchResultAndMigrateLiveData`;

CREATE DEFINER=`user`@`%` PROCEDURE `Match_UpdateMatchResultAndMigrateLiveData`(
	IN sportId INT, 
    IN matchId VARCHAR(45), 
    IN matchResult TEXT)
BEGIN	
   IF EXISTS (SELECT 1 FROM `LiveMatch` WHERE SportId = sportId AND Id = matchId) 
   THEN
		UPDATE `Match` as M
		SET  M.`Value` = 
				(SELECT `Value` FROM `LiveMatch` as LM 
				WHERE LM.SportId = sportId
				AND LM.Id = matchId)
		WHERE M.SportId = sportId
			AND M.Id = matchId;
	END IF;
    
	UPDATE `Match` as M
    SET  M.`Value` = JSON_REPLACE(Value,  '$.MatchResult', JSON_EXTRACT(matchResult, '$'))
	WHERE M.SportId = sportId
		AND M.Id = matchId;
END
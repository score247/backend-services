DROP procedure IF EXISTS `Match_UpdateMatchResultAndMigrateLiveData`;

CREATE DEFINER=`user`@`%` PROCEDURE `Match_UpdateMatchResultAndMigrateLiveData`(
	IN sportId INT, 
    IN matchId VARCHAR(45), 
    IN matchResult TEXT)
BEGIN	
   IF EXISTS (SELECT * FROM `LiveMatch` WHERE SportId = sportId AND Id = matchId FOR SHARE) 
   THEN
		UPDATE `LiveMatch` as LM
		SET  LM.`Value` = JSON_REPLACE(Value,  '$.MatchResult', JSON_EXTRACT(matchResult, '$'))
		WHERE LM.SportId = sportId AND LM.Id = matchId;
        
		UPDATE `Match` as M
		SET  M.`Value` = 
				(SELECT `Value` FROM `LiveMatch` as LM 
				WHERE LM.SportId = sportId
				AND LM.Id = matchId)
		WHERE M.SportId = sportId
			AND M.Id = matchId;
	END IF;
END
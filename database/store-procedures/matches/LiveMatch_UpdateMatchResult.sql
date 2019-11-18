DROP procedure IF EXISTS `LiveMatch_UpdateMatchResult`;

CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_UpdateMatchResult`(
	IN sportId INT(11), 
    IN matchId VARCHAR(45), 
    IN matchResult TEXT)
BEGIN
			UPDATE `LiveMatch` as LM
			SET `Value` = JSON_SET(`Value`,  '$.MatchResult', JSON_EXTRACT(matchResult, '$'))
			WHERE LM.`SportId` = sportId AND LM.Id = matchId;	 
END
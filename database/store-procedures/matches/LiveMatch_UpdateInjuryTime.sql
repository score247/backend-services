DROP procedure IF EXISTS `LiveMatch_UpdateInjuryTime`;

CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_UpdateInjuryTime`(
	IN sportId INT(11), 
    IN matchId VARCHAR(45), 
    IN injuryTimes TEXT)
BEGIN
	UPDATE `LiveMatch` as LM
	SET `Value` = JSON_SET(`Value`,  '$.InjuryTimes', JSON_EXTRACT(injuryTimes, '$'))
	WHERE LM.`SportId` = sportId 
	AND LM.Id = matchId;
    
    UPDATE `Match` as M
	SET `Value` = JSON_SET(`Value`,  '$.InjuryTimes', JSON_EXTRACT(injuryTimes, '$'))
	WHERE M.`SportId` = sportId 
	AND M.Id = matchId;
END
DROP procedure IF EXISTS `LiveMatch_UpdateCurrentPeriodStartTime`;

CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_UpdateCurrentPeriodStartTime`(IN sportId INT, IN matchId VARCHAR(45), IN currentPeriodStartTime TINYTEXT)
BEGIN
	UPDATE `LiveMatch` as LM
	SET `Value` = JSON_SET(`Value`,  '$.CurrentPeriodStartTime', JSON_UNQUOTE(currentPeriodStartTime))
	WHERE LM.`SportId` = sportId AND LM.Id = matchId;
	
	UPDATE `Match` as M
	SET `Value` = JSON_SET(`Value`,  '$.CurrentPeriodStartTime', JSON_UNQUOTE(currentPeriodStartTime))
	WHERE M.`SportId` = sportId AND M.Id = matchId;
END
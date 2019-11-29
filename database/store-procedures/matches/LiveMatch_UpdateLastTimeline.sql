DROP procedure IF EXISTS `LiveMatch_UpdateLastTimeline`;

CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_UpdateLastTimeline`(
	IN sportId INT(11), 
    IN matchId VARCHAR(45), 
    IN timelineEvent TEXT)
BEGIN
	UPDATE `LiveMatch` as LM
	SET `Value` = JSON_SET(`Value`,  '$.LatestTimeline', JSON_EXTRACT(timelineEvent, '$'))
	WHERE LM.`SportId` = sportId 
	AND LM.Id = matchId;
    
    UPDATE `Match` as M
	SET `Value` = JSON_SET(`Value`,  '$.LatestTimeline', JSON_EXTRACT(timelineEvent, '$'))
	WHERE M.`SportId` = sportId 
	AND M.Id = matchId;
END
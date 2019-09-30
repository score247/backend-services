DROP procedure IF EXISTS `LiveMatch_UpdateMatchResult`;

CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_UpdateMatchResult`(
	IN sportId INT(11), 
    IN matchId VARCHAR(45), 
    IN matchResult TEXT)
BEGIN
	IF EXISTS (SELECT * FROM `Match` AS M WHERE M.Id = matchId FOR SHARE) 
    THEN
		 IF EXISTS (SELECT * FROM `LiveMatch` AS LM WHERE LM.Id = matchId FOR SHARE) 
         THEN
			UPDATE `LiveMatch` as LM
			SET `Value` = JSON_SET(`Value`,  '$.MatchResult', JSON_EXTRACT(matchResult, '$'))
			WHERE LM.`SportId` = sportId AND LM.Id = matchId;
		 ELSE
			 INSERT INTO `LiveMatch`
			 SELECT 
				Id, 
				JSON_SET(`Value`,  '$.MatchResult', JSON_EXTRACT(matchResult, '$')) as `Value`,
				`Language`,
				`SportId`,
				LeagueId,
				EventDate,
				Region,
				now(),
				now()
				FROM `Match` as M
			 WHERE M.`SportId` = sportId AND M.Id = matchId;
		 END IF;
    END IF;
END
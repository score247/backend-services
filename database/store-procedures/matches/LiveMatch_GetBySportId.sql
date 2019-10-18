DROP procedure IF EXISTS `LiveMatch_GetBySportId`;

CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_GetBySportId`(IN sportId INT, IN language TEXT)
BEGIN
	SELECT JSON_REPLACE(LM.Value, '$.ModifiedTime', LM.ModifiedTime) as `Value`
    FROM LiveMatch as LM 
    INNER JOIN `League` as League ON LM.LeagueId = League.Id 	
	WHERE LM.SportId = sportId 
		AND LM.`Language` = language 
        AND League.IsActive = 1
    ORDER BY League.Order, LM.EventDate, LM.LeagueId, LM.Id;
END
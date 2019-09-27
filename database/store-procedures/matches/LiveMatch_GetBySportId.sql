DROP procedure IF EXISTS `LiveMatch_GetBySportId`;

CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_GetBySportId`(IN sportId INT, IN language TEXT)
BEGIN
	SELECT `Value`
    FROM LiveMatch as LM 
    INNER JOIN `League` as League ON `Match`.LeagueId = League.Id 	
	WHERE LM.SportId = sportId 
		AND LM.`Language` = language 
        AND League.IsActive = 1
    ORDER BY League.Order, `Match`.EventDate, `Match`.LeagueId, `Match`.Id;
END
DROP procedure IF EXISTS `Match_GetByLeagueId`;

CREATE DEFINER=`user`@`%` PROCEDURE `Match_GetByLeagueId`(IN sportId INT, IN leagueId varchar(45), IN language TEXT)
BEGIN
	SELECT M.`Value`
		FROM `Match` as M  
		WHERE M.LeagueId = leagueId
		AND M.SportId = sportId
        AND M.Language = language; 
END
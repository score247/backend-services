DROP procedure IF EXISTS `Admin_League_GetLeagueSeasonId`;

CREATE DEFINER=`user`@`%` PROCEDURE `Admin_League_GetLeagueSeasonId`(IN leagueId VARCHAR(45))
BEGIN
	SELECT LS.SeasonId FROM `LeagueSeason` as LS WHERE LS.LeagueId = leagueId;
END
DROP procedure IF EXISTS `Admin_League_RemoveLeagueSeason`;

CREATE DEFINER=`user`@`%` PROCEDURE `Admin_League_RemoveLeagueSeason`(IN leagueId varchar(45))
BEGIN
	DELETE FROM `LeagueSeason` as LS WHERE LS.LeagueId = leagueId;
END
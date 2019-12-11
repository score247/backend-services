DROP procedure IF EXISTS `League_GetUnprocessedLeagueSeason`;

CREATE DEFINER=`user`@`%` PROCEDURE `League_GetUnprocessedLeagueSeason`()
BEGIN
	SELECT  Season.`LeagueId`,
			Season.`SeasonId`,
			Season.`Region`,
			Season.`Fetched`,
			Season.`FetchedDate`
		FROM LeagueSeason AS Season 
		INNER JOIN League AS League ON Season.LeagueId = League.Id
		WHERE League.IsMajor = 1
		AND Season.Fetched = 0;
END
DROP procedure IF EXISTS `League_GetLeagueSeason`;

CREATE DEFINER=`user`@`%` PROCEDURE `League_GetLeagueSeason`()
BEGIN
	SELECT  Season.`LeagueId`,
			Season.`SeasonId`,
			Season.`Region`,
			Season.`Fetched`,
			Season.`FetchedDate`,
			Season.`CreatedTime`,
			Season.`ModifiedTime`
		FROM LeagueSeason AS Season 
		INNER JOIN League AS League ON Season.LeagueId = League.Id
		WHERE League.IsActive = 1
		AND Season.Fetched = 0;
END
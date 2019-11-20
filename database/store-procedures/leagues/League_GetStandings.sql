DROP procedure IF EXISTS `League_GetStandings`;

CREATE PROCEDURE `League_GetStandings`(
	IN leagueId VARCHAR(45),
    IN seasonId VARCHAR(45),
    IN language TEXT)
BEGIN
	SELECT `Value` FROM `Standing` as SD 
		WHERE SD.LeagueId = leagueId
			AND SD.SeasonId = seasonId
			AND SD.`Language` = language;
END
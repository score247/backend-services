DROP procedure IF EXISTS `League_GetStandings`;

CREATE PROCEDURE `League_GetStandings`(
	IN leagueId VARCHAR(45),
    IN seasonId VARCHAR(45),
    IN tableType VARCHAR(10),
    IN language VARCHAR(10))
BEGIN
	SELECT `Value` FROM `Standing` as SD 
		WHERE SD.LeagueId = leagueId
			AND SD.SeasonId = seasonId
			AND SD.TableType = tableType
			AND SD.`Language` = language;
END
DROP procedure IF EXISTS `League_GetGroupStages`;

CREATE PROCEDURE `League_GetGroupStages`(IN leagueId VARCHAR(45), IN seasonId VARCHAR(45), IN languageCode TEXT)
BEGIN
	SELECT LGS.Value 
    FROM `LeagueGroupStage` as LGS 
    WHERE LGS.LeagueId = leagueId 
		AND LGS.LeagueSeasonId = seasonId
        AND LGS.Language = languageCode;
END
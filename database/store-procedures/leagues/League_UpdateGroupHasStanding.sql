DROP procedure IF EXISTS `League_UpdateGroupHasStanding`;

CREATE PROCEDURE `League_UpdateGroupHasStanding` (
	IN leagueId VARCHAR(45), 
	IN seasonId VARCHAR(45),
	IN leagueGroups TEXT, 
	IN language TEXT)
BEGIN
	UPDATE `LeagueGroupStage`
    SET 
		`HasStanding` = true,
        `Value` = JSON_REPLACE(`Value`, '$.HasStanding', 'true')
    WHERE `LeagueId` = leagueId
		AND `SeasonId` = `LeagueSeasonId`
        AND JSON_SEARCH(leagueGroups,'one', JSON_UNQUOTE(JSON_EXTRACT(`Value`, '$.LeagueRound.Group'))) is not null;
END

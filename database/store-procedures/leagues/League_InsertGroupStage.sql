DROP procedure IF EXISTS `League_InsertGroupStage`;

CREATE DEFINER=`user`@`%` PROCEDURE `League_InsertGroupStage`(
	IN leagueId varchar(45),
    IN leagueSeasonId varchar(45),
    IN groupStageName varchar(250),
	IN groupName varchar(250),
    IN hasStanding boolean,
    IN groupStageValue MEDIUMTEXT,
    IN language varchar(10))
BEGIN
	INSERT INTO `LeagueGroupStage`
		(`LeagueId`,
		`LeagueSeasonId`,
		`GroupStageName`,
		`GroupName`,
		`HasStanding`,
		`Value`,
		`Language`,
		`CreatedTime`,
		`ModifiedTime`)
	VALUES
		(leagueId,
		leagueSeasonId,
		groupStageName,
		groupName,
		hasStanding,
		groupStageValue,
		language,
		now(),
		now()
		)
	ON DUPLICATE KEY UPDATE
		`HasStanding` = IF(hasStanding = true,  hasStanding, `HasStanding`),
		`Value` = IF(hasStanding = true,  groupStageValue, `Value`),
        `GroupName` = groupName,
		`ModifiedTime` = now();
END
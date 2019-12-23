DROP procedure IF EXISTS `League_InsertGroupStage`;

CREATE DEFINER=`user`@`%` PROCEDURE `League_InsertGroupStage`(
	IN leagueId varchar(45),
    IN leagueSeasonId varchar(45),
    IN groupStageName varchar(250),
    IN groupStageValue MEDIUMTEXT,
    IN language varchar(10))
BEGIN
INSERT INTO `LeagueGroupStage`
	(`LeagueId`,
	`LeagueSeasonId`,
	`GroupStageName`,
    `Value`,
	`Language`,
	`CreatedTime`,
	`ModifiedTime`)
VALUES
	(leagueId,
	leagueSeasonId,
    groupStageName,
    groupStageValue,
	language,
    now(),
    now()
	)
ON DUPLICATE KEY UPDATE
	`Value` = groupStageValue,
	`ModifiedTime` = now();
END
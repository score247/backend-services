DROP procedure IF EXISTS `League_InsertGroupStage`;

CREATE DEFINER=`user`@`%` PROCEDURE `League_InsertGroupStage`(IN leagueId varchar(45), IN leagueGroupName varchar(250), IN language varchar(10))
BEGIN
INSERT INTO `LeagueGroupStage`
	(`LeagueId`,
	`GroupStageName`,
	`Language`,
	`CreatedTime`,
	`ModifiedTime`)
VALUES
	(leagueId,
	leagueGroupName,
	language,
    now(),
    now()
	);
END
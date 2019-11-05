DROP PROCEDURE IF EXISTS `Team_GetTeamResults`;
CREATE DEFINER=`user`@`%` PROCEDURE `Team_GetTeamResults`(IN teamId TINYTEXT, IN `language` TINYTEXT)
BEGIN
	SELECT `Value` FROM HeadToHead as H2H
    WHERE H2H.HomeTeamId = teamId OR H2H.AwayTeamId = teamId AND H2H.Language = `language`;
END
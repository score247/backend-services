DROP PROCEDURE IF EXISTS `Team_GetHeadToHeads`;

CREATE DEFINER=`user`@`%` PROCEDURE `Team_GetHeadToHeads`(IN homeTeamId TINYTEXT, IN awayTeamId TINYTEXT, IN `language` TINYTEXT)
BEGIN
	SELECT `Value` FROM HeadToHead as H2H
    WHERE H2H.HomeTeamId = homeTeamId AND H2H.AwayTeamId = awayTeamId AND H2H.Language = `language`;
END
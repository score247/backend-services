DROP PROCEDURE IF EXISTS `Team_InsertOrUpdateHeadToHead`;

CREATE DEFINER=`user`@`%` PROCEDURE `Team_InsertOrUpdateHeadToHead`(IN homeTeamId TINYTEXT, IN awayTeamId TINYTEXT, IN `match` MEDIUMTEXT, IN matchId TINYTEXT, IN `language` TINYTEXT)
BEGIN
	INSERT INTO `HeadToHead`
    VALUES(
		homeTeamId, 
        awayTeamId, 
        matchId, 
        `language`,
        `match`,
        now(),
		now())
    ON DUPLICATE KEY
    UPDATE `Value` = `match`;
END
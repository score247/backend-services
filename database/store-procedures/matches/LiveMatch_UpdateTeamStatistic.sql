DROP procedure IF EXISTS `LiveMatch_UpdateTeamStatistic`;

CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_UpdateTeamStatistic`(IN sportId INT, IN matchId VARCHAR(45), IN teamIndex INT, IN statistic TEXT)
BEGIN
	UPDATE `LiveMatch` as LM
    SET Value = JSON_SET(Value, CONCAT('$.Teams[', teamIndex ,'].Statistic'), JSON_EXTRACT(statistic, '$'))
    WHERE LM.`SportId` = sportId AND LM.`Id` = matchId;
END
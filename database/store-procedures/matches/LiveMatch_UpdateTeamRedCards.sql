CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_UpdateTeamRedCards`(IN sportId INT, IN matchId VARCHAR(45), IN teamIndex INT, IN redCards INT)
BEGIN
	UPDATE `LiveMatch` as LM
    SET Value = JSON_SET(Value, CONCAT('$.Teams[', teamIndex ,'].Statistic.RedCards'), redCards)
    WHERE LM.`SportId` = sportId AND LM.`Id` = matchId;
END
DROP procedure IF EXISTS `LiveMatch_UpdateTeamCards`;

CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_UpdateTeamCards`(
			IN sportId INT, 
            IN matchId VARCHAR(45), 
            IN teamIndex INT, 
            IN redCards INT,
            IN yellowRedCards INT,
			IN yellowCards INT)
BEGIN
	UPDATE `LiveMatch` as LM
    SET Value = JSON_SET(Value, 
			CONCAT('$.Teams[', teamIndex ,'].Statistic.RedCards'), redCards,
            CONCAT('$.Teams[', teamIndex ,'].Statistic.YellowRedCards'), yellowRedCards,
			CONCAT('$.Teams[', teamIndex ,'].Statistic.YellowCards'), yellowCards)
    WHERE LM.`SportId` = sportId AND LM.`Id` = matchId;
    
    UPDATE `Match` as M
    SET Value = JSON_SET(Value, 
			CONCAT('$.Teams[', teamIndex ,'].Statistic.RedCards'), redCards,
            CONCAT('$.Teams[', teamIndex ,'].Statistic.YellowRedCards'), yellowRedCards,
			CONCAT('$.Teams[', teamIndex ,'].Statistic.YellowCards'), yellowCards)
    WHERE M.`SportId` = sportId AND M.`Id` = matchId;
END
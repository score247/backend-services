DROP procedure IF EXISTS `Admin_League_UpdateActiveStatus`;

CREATE DEFINER=`user`@`%` PROCEDURE `Admin_League_UpdateActiveStatus`(IN leagueId varchar(45), IN isActive tinyint)
BEGIN
	UPDATE `League` SET `IsActive` = isActive WHERE `Id` = leagueId;
END
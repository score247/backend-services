DROP procedure IF EXISTS `League_GetById`;

CREATE DEFINER=`user`@`%` PROCEDURE `League_GetById`(IN leagueId varchar(255))
BEGIN
	SELECT * FROM `League` WHERE `Id` = leagueId;
END
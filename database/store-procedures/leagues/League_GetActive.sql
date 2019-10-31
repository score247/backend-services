DROP procedure IF EXISTS `League_GetActive`;

CREATE DEFINER=`user`@`%` PROCEDURE `League_GetActive`()
BEGIN
	SELECT `Id`, `Name`, `Order`, `Country` as 'CountryName', `CountryCode` FROM `League` WHERE `IsActive` = '1';
END
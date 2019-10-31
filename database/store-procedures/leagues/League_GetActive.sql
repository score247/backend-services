DROP procedure IF EXISTS `League_GetActive`;

CREATE DEFINER=`user`@`%` PROCEDURE `League_GetActive`()
BEGIN
	SELECT `Id`, `Name`, `Order`, `CategoryId`, `Country` as 'CountryName', `CountryCode`, `IsInternational`, `Region` FROM `League` WHERE `IsActive` = '1';
END
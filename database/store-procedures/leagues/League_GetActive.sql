DROP procedure IF EXISTS `League_GetActive`;

CREATE DEFINER=`user`@`%` PROCEDURE `League_GetActive`(IN languageCode TINYTEXT)
BEGIN
	SELECT 
		`Id`, 
		`Name`,
        `Order`, 
        `CategoryId`,
        `Country` as 'CountryName', 
        `CountryCode`, 
        `IsInternational`, 
        `Region` 
        FROM `League` as L
        WHERE L.`IsActive` = '1' AND L.`Language` = languageCode;
END
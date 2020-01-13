DROP procedure IF EXISTS `League_GetCountryLeagues`;

CREATE DEFINER=`user`@`%` PROCEDURE `League_GetCountryLeagues`(IN countryCode VARCHAR(45), IN isInternational bool, IN languageCode TINYTEXT)
BEGIN
	SELECT 
		L.`Id`, 
		L.`Name`,
        L.`Order`, 
        L.`CategoryId`,
        L.`Country` as 'CountryName', 
        L.`CountryCode`, 
        L.`IsInternational`, 
        L.`Region`,
        L.`CurrentSeasonId`,
		NULL as 'SeasonDates',
		IF(L.`HasGroups` = 1, true, false)  AS 'HasGroups',
		L.`Abbreviation`
        FROM `League` as L
        WHERE L.IsActive = '1' 
			AND ((isInternational = true AND L.IsInternational = true)
				OR (isInternational = false AND L.CountryCode = countryCode))
            AND L.Language = languageCode;
END
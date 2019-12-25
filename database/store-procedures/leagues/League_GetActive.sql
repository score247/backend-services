DROP procedure IF EXISTS `League_GetActive`;

CREATE DEFINER=`user`@`%` PROCEDURE `League_GetActive`(IN languageCode TINYTEXT)
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
        LS.`SeasonDates`
        FROM `League` as L
			LEFT JOIN `LeagueSeason` as LS
			ON LS.`SeasonId` = L.`CurrentSeasonId`
        WHERE L.`IsActive` = '1' AND L.`Language` = languageCode;
END
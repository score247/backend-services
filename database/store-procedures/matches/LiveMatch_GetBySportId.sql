DROP procedure IF EXISTS `LiveMatch_GetBySportId`;

CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_GetBySportId`(IN sportId INT, IN language TEXT, IN fromDate DATETIME)
BEGIN
	SELECT `Value`
    FROM LiveMatch as LM
	WHERE LM.SportId = sportId
		AND LM.`Language` = language
        AND LM.EventDate >= fromDate;
END
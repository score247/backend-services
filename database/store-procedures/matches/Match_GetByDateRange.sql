DROP procedure IF EXISTS `Match_GetByDateRange`;

CREATE DEFINER=`user`@`%` PROCEDURE `Match_GetByDateRange`(IN sportId INT, IN fromDate DATETIME, IN toDate DATETIME, IN language TEXT)
BEGIN
	SELECT JSON_REPLACE(M.Value,  '$.ModifiedTime', M.ModifiedTime) as `Value` FROM `Match` as M 	
		 WHERE M.SportId = sportId
			AND  M.EventDate >=  fromDate
			AND M.EventDate <=  toDate
			AND M.`Language` = language;
END
CREATE DEFINER=`user`@`%` PROCEDURE `Score247_GetMatchesByDateRange`(in sportId int, in fromDate datetime, in toDate datetime, in language text)
BEGIN
	SELECT `Value` FROM score247_db2.`Match`
	WHERE SportId = sportId
		AND  EventDate >=  fromDate
		AND EventDate <=  toDate
		AND Language = language;
END
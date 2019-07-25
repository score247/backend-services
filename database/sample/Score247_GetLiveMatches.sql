CREATE DEFINER=`user`@`%` PROCEDURE `Score247_GetLiveMatches`(in sportId int, in language text)
BEGIN
	SELECT `Value` FROM score247_db2.LiveMatch
		 WHERE SportId = sportId AND `Language` = language;
END
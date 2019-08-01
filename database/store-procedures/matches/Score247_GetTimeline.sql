CREATE DEFINER=`user`@`%` PROCEDURE `Score247_GetTimeline`(IN matchId VARCHAR(50))
BEGIN
	SELECT `Value` FROM Timeline
    WHERE MatchId = matchId;
END
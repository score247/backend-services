DROP procedure IF EXISTS `Match_GetTimelineEvents`;

CREATE DEFINER=`user`@`%` PROCEDURE `Match_GetTimelineEvents`(IN matchId VARCHAR(45))
BEGIN
	SELECT `Value` 
    FROM Timeline AS T
    WHERE T.MatchId = matchId;
END
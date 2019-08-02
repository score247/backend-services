CREATE DEFINER=`dev`@`%` PROCEDURE `Odds_GetOdds`(
	IN matchId VARCHAR(45), 
    IN betTypeId INT,
    IN bookmakerId VARCHAR(20))
BEGIN
	SELECT `Value` 
    FROM Odds
    WHERE MatchId = matchId
		AND BetTypeId = betTypeId
        AND BookmakerId = bookmakerId;
END
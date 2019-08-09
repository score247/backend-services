CREATE DEFINER=`user`@`%` PROCEDURE `Odds_GetOdds`(
	IN matchId VARCHAR(45), 
    IN betTypeId INT,
    IN bookmakerId VARCHAR(20))
BEGIN
	SELECT `Value` 
    FROM Odds AS o
    WHERE o.MatchId = matchId
		AND (betTypeId = 0 OR o.BetTypeId = betTypeId)
        AND (bookmakerId = '' OR o.BookmakerId = bookmakerId);
END
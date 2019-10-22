DROP procedure IF EXISTS `Match_GetLineups`;

CREATE PROCEDURE `Match_GetLineups` (IN matchId TEXT, IN language TEXT)
BEGIN
	SELECT `Value` FROM `Lineups` as LU  WHERE LU.MatchId = matchId AND LU.`Language` = language;
END

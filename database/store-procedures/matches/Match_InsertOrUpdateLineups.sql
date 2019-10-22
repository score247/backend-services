DROP procedure IF EXISTS `Match_InsertOrUpdateLineups`;

CREATE PROCEDURE `Match_InsertOrUpdateLineups` (IN matchId TEXT, IN lineups TEXT, IN language TEXT)
BEGIN
	 INSERT INTO `Lineups` VALUES (
			matchId,
			JSON_UNQUOTE(JSON_EXTRACT(lineups, '$')),
            language,
            now(),
            now())
	 ON DUPLICATE KEY UPDATE
			`Value` = JSON_UNQUOTE(JSON_EXTRACT(lineups, '$')),
			ModifiedTime = now();
END
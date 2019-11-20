DROP procedure IF EXISTS `League_InsertOrUpdateStandings`;

CREATE PROCEDURE `League_InsertOrUpdateStandings`(IN leagueId VARCHAR(45), IN seasonId VARCHAR(45), IN standings TEXT, IN language TEXT)
BEGIN
	 INSERT INTO `Standings` VALUES (
			leagueId,
			seasonId,
            language,
			JSON_UNQUOTE(JSON_EXTRACT(standings, '$')),
            now(),
            now())
	 ON DUPLICATE KEY UPDATE
			`Value` = JSON_UNQUOTE(JSON_EXTRACT(standings, '$')),
			ModifiedTime = now();
END
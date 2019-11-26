DROP procedure IF EXISTS `Match_InsertOrUpdateLineups`;

CREATE DEFINER=`user`@`%` PROCEDURE `Match_InsertOrUpdateLineups`(IN matchId TEXT, IN lineups TEXT, IN language TEXT)
BEGIN
	 INSERT INTO `Lineups` 
     (`MatchId`,
		`Value`,
		`Language`,
		`CreatedTime`,
		`ModifiedTime`,
		`EventDate`)
     VALUES (
			matchId,
			JSON_UNQUOTE(JSON_EXTRACT(lineups, '$')),
            language,
            now(),
            now(),
            STR_TO_DATE(JSON_UNQUOTE(JSON_EXTRACT(lineups, '$.EventDate')),'%Y-%m-%dT%H:%i:%s+00:00'))
	 ON DUPLICATE KEY UPDATE
			`Value` = JSON_UNQUOTE(JSON_EXTRACT(lineups, '$')),
			ModifiedTime = now();
END
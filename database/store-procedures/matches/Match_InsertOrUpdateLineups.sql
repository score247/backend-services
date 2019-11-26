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
            JSON_UNQUOTE(JSON_EXTRACT(lineups, '$.EventDate')))
	 ON DUPLICATE KEY UPDATE
			`Value` = JSON_UNQUOTE(JSON_EXTRACT(lineups, '$')),
			ModifiedTime = now();
END
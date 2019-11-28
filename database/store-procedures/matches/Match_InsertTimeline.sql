DROP procedure IF EXISTS `Match_InsertTimeline`;

CREATE DEFINER=`user`@`%` PROCEDURE `Match_InsertTimeline`(IN matchId TEXT, IN timeline TEXT, IN language TEXT)
BEGIN
	 INSERT INTO `Timeline` 
     (`Id`,
	`MatchId`,
	`Value`,
	`Language`,
	`CreatedTime`,
	`ModifiedTime`,
	`EventDate`,
    `Type`)
     VALUES (
			JSON_UNQUOTE(JSON_EXTRACT(timeline, '$.Id')),
			matchId,
			JSON_UNQUOTE(JSON_EXTRACT(timeline, '$')),
            language,
            now(),
            now(),
            (SELECT M.EventDate FROM `Match` AS M WHERE M.Id = matchId LIMIT 1),
            JSON_UNQUOTE(JSON_EXTRACT(timeline, '$.Type.DisplayName')))
	 ON DUPLICATE KEY UPDATE
			`Value` = JSON_UNQUOTE(JSON_EXTRACT(timeline, '$')),
            `Type` = JSON_UNQUOTE(JSON_EXTRACT(timeline, '$.Type.DisplayName')),
			ModifiedTime = now();
END
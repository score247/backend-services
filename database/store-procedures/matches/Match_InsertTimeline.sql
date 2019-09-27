DROP procedure IF EXISTS `Match_InsertTimeline`;

CREATE DEFINER=`user`@`%` PROCEDURE `Match_InsertTimeline`(IN matchId TEXT, IN timeline TEXT, IN language TEXT)
BEGIN
	 INSERT INTO `Timeline` VALUES (
			JSON_UNQUOTE(JSON_EXTRACT(timeline, '$.Id')),
			matchId,
			JSON_UNQUOTE(JSON_EXTRACT(timeline, '$')),
            language,
            now(),
            now())
	 ON DUPLICATE KEY UPDATE
			`Value` = JSON_UNQUOTE(JSON_EXTRACT(timeline, '$')),
			ModifiedTime = now();
END
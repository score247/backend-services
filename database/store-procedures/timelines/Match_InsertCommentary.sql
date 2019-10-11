DROP procedure IF EXISTS `Match_InsertCommentary`;

CREATE DEFINER=`user`@`%` PROCEDURE `Match_InsertCommentary`(IN matchId TEXT, IN timelineId BIGINT, IN commentaries TEXT, IN language TEXT)
BEGIN
	 INSERT INTO `Commentary` (
		`MatchId`,
        `TimelineId`,
        `Value`,
        `Language`,
        `CreatedTime`,
        `ModifiedTime`
		)
		VALUES (			
			matchId,
            timelineId,
			JSON_UNQUOTE(JSON_EXTRACT(commentaries, '$')),
            language,
            now(),
            now());
END
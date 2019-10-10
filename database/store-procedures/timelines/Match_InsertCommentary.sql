DROP procedure IF EXISTS `Match_InsertCommentary`;

CREATE DEFINER=`user`@`%` PROCEDURE `Match_InsertCommentary`(IN matchId TEXT, IN timelineId BIGINT, IN commentaries TEXT, IN language TEXT)
BEGIN
	IF NOT EXISTS (SELECT 1 
						FROM `Commentary` AS Commentary 
						WHERE  Commentary.MatchId = matchId 
							AND Commentary.TimelineId = timelineId 
                            AND Commentary.Language = language)
    THEN        
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
	ELSE
			UPDATE `Commentary` 
			SET 
				`Value` = JSON_SET(`Value`,  '$', JSON_EXTRACT(commentaries, '$')),
                `ModifiedTime` = now()
			WHERE MatchId = matchId
				AND TimelineId = timelineId 
				AND Language = language;		
    END IF;
END
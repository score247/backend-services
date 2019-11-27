DROP procedure IF EXISTS `Match_InsertCommentary`;

CREATE DEFINER=`user`@`%` PROCEDURE `Match_InsertCommentary`(IN matchId TEXT, IN timelineId BIGINT, IN commentaries TEXT, IN language TEXT)
BEGIN
	IF NOT EXISTS (SELECT 1 
						FROM `Commentary` AS Commentary 
						WHERE  Commentary.MatchId = matchId 
							AND Commentary.TimelineId = timelineId 
                            AND Commentary.Language = language)
    THEN        
		 INSERT INTO `Commentary` 
         (`TimelineId`,
			`MatchId`,
			`Value`,
			`Language`,
			`CreatedTime`,
			`ModifiedTime`,
			`EventDate`)
			VALUES (	
				timelineId,			
				matchId,				
				JSON_UNQUOTE(JSON_EXTRACT(commentaries, '$')),
				language,
				now(),
				now(),
                (SELECT M.EventDate FROM `Match` AS M WHERE M.Id = matchId LIMIT 1));
	ELSE
			UPDATE `Commentary` AS Commentary
			SET 
				Commentary.`Value` = JSON_SET(`Value`,  '$', JSON_EXTRACT(commentaries, '$')),
                Commentary.`ModifiedTime` = now()
			WHERE Commentary.MatchId = matchId
				AND Commentary.TimelineId = timelineId 
				AND Commentary.Language = language;		
    END IF;
END
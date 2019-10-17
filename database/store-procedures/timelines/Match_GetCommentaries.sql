DROP procedure IF EXISTS `Match_GetCommentaries`;

CREATE DEFINER=`user`@`%` PROCEDURE `Match_GetCommentaries`(IN matchId VARCHAR(45), IN language VARCHAR(10))
BEGIN
	SELECT JSON_SET(T.`Value`,  '$.Commentaries', C.`Value`) AS Commentary
    FROM Timeline AS T
    LEFT JOIN Commentary AS C ON 		
		T.Id = C.TimelineId 
        AND T.MatchId = C.MatchId
    WHERE T.MatchId = matchId
		AND T.Language = language;
END
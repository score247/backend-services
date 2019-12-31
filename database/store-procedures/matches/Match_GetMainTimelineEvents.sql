DROP procedure IF EXISTS `Match_GetMainTimelineEvents`;

CREATE DEFINER=`user`@`%` PROCEDURE `Match_GetMainTimelineEvents`(IN matchId VARCHAR(45), IN language VARCHAR(10))
BEGIN
	SELECT JSON_SET(M.`Value`,  '$.TimeLines', (SELECT JSON_ARRAYAGG(T.`Value`)  as Timelines
		FROM Timeline AS T 
		WHERE T.MatchId = matchId
        AND T.Language = language
		AND T.`Type` IN ('score_change', 'penalty_missed', 'yellow_card', 'red_card', 'yellow_red_card', 'penalty_shootout', 'break_start', 'match_ended', 'substitution'))	
	) AS Value
	FROM `Match` AS M
	WHERE M.Id = matchId
    AND M.Language = language;
END
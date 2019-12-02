DROP procedure IF EXISTS `Match_RemoveTimelines`;

CREATE DEFINER=`user`@`%` PROCEDURE `Match_RemoveTimelines`(
	IN matchId varchar(45),
    IN timelineIds TEXT)
BEGIN
    DECLARE i INT DEFAULT 0;     
    DECLARE e INT DEFAULT JSON_LENGTH(timelineIds);
    
    WHILE i < e DO 
		SET @DeleteId = JSON_UNQUOTE(JSON_EXTRACT(removedMatchIds, CONCAT('$[', i, '].Id')));         
		DELETE FROM `Timeline` WHERE Id = @DeleteId AND MatchId = matchId;         
        -- Increment the loop variable                                                                                                                                                        
        SET i = i + 1;
    END WHILE;     
END
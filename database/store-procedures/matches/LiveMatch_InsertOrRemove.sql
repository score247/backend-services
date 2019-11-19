DROP procedure IF EXISTS `LiveMatch_InsertOrRemove`;

CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_InsertOrRemove`(
	IN sportId INT, 
    IN language TEXT, 
    IN newMatches MEDIUMTEXT, 
    IN removedMatchIds TEXT)
BEGIN
	DECLARE ni INT DEFAULT 0;     
    DECLARE ne INT DEFAULT JSON_LENGTH(newMatches);
    
    DECLARE di INT DEFAULT 0;     
    DECLARE de INT DEFAULT JSON_LENGTH(removedMatchIds);
    
    -- Insert new live matches
    WHILE ni < ne DO          
		SET @Id = JSON_UNQUOTE(JSON_EXTRACT(newMatches, CONCAT('$[', ni, '].MatchId')));
    
		IF NOT EXISTS (SELECT 1 FROM `LiveMatch` WHERE Id = @Id AND Language = language) 
        THEN			
			 INSERT INTO `LiveMatch`
             (`Id`, `Value`, `SportId`, `Language`, `LeagueId`, `EventDate`, `Region`, `LeagueSeasonId`, `CreatedTime`, `ModifiedTime`)
			 SELECT 
				Id, 
				JSON_SET(`Value`,  '$.MatchResult', JSON_EXTRACT(newMatches, CONCAT('$[', ni, '].MatchResult'))) as `Value`,
				`Language`,
				`SportId`,
				LeagueId,
				EventDate,
				Region,
                JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].LeagueSeason.Id'))),
				now(),
				now()
				FROM `Match` as M
			 WHERE M.`SportId` = sportId 
				AND M.Id = @Id
                AND M.Language = language;
		 END IF;
         
        -- Increment the loop variable                                                                                                                                                        
        SET ni = ni + 1;
    END WHILE; 
    
    -- Delete live matches
    WHILE di < de DO 
		SET @DeleteId = JSON_UNQUOTE(JSON_EXTRACT(removedMatchIds, CONCAT('$[', di, '].MatchId')));         
		DELETE FROM `LiveMatch` WHERE Id = @DeleteId AND SportId = sportId AND Language = language;         
        -- Increment the loop variable                                                                                                                                                        
        SET di = di + 1;
    END WHILE;     
END
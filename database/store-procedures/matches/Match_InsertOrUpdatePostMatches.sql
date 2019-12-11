DROP procedure IF EXISTS `Match_InsertOrUpdatePostMatches`;

CREATE DEFINER=`root`@`%` PROCEDURE `Match_InsertOrUpdatePostMatches`(IN sportId INT, IN matches MEDIUMTEXT, IN language TEXT)
BEGIN
	DECLARE i INT DEFAULT 0;                                                                                                                                                    
    DECLARE e INT DEFAULT JSON_LENGTH(matches);
	DECLARE Id VARCHAR(45);

    WHILE i < e DO 
    
			SET @Id = JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].Id')));
            
            INSERT INTO `Match` 
			(`Id`, `Value`, `SportId`, `Language`, `LeagueId`, `EventDate`, `Region`, `LeagueSeasonId`, `CreatedTime`, `ModifiedTime`)
			VALUES (
				JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].Id'))),
				JSON_EXTRACT(matches, CONCAT('$[', i, ']')),            
				sportId, 
				language, 
				JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].League.Id'))), 
				STR_TO_DATE(JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].EventDate'))),'%Y-%m-%dT%H:%i:%s+00:00'),
				JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].Region'))),
				JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].LeagueSeason.Id'))),
				now(),
				now())
			ON DUPLICATE KEY UPDATE
				Value = JSON_REPLACE(Value,  '$.MatchResult', JSON_EXTRACT(matches, CONCAT('$[', i, '].MatchResult'))),
				ModifiedTime = now();
            
        -- Increment the loop variable                                                                                                                                                        
        SET i = i + 1;        
      
    END WHILE;
END
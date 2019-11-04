DROP procedure IF EXISTS `Match_InsertOrUpdatePostMatches`;

CREATE DEFINER=`root`@`%` PROCEDURE `Match_InsertOrUpdatePostMatches`(IN sportId INT, IN matches MEDIUMTEXT, IN language TEXT)
BEGIN
	DECLARE i INT DEFAULT 0;                                                                                                                                                    
    DECLARE e INT DEFAULT JSON_LENGTH(matches);
	DECLARE Id VARCHAR(45);

    WHILE i < e DO        
        
			SET @Id = JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].Id')));
            
			IF(NOT EXISTS (SELECT 1 FROM  `Match` AS M WHERE M.Id = @Id AND M.Language = language))
			THEN
				INSERT INTO `Match` VALUES (
					@Id,
					JSON_EXTRACT(matches, CONCAT('$[', i, ']')),
					language, 
					sportId, 
					JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].League.Id'))), 
					STR_TO_DATE(JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].EventDate'))),'%Y-%m-%dT%H:%i:%s+00:00'),
					JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].Region'))),
					now(),
					now());
			ELSE
				UPDATE `Match` as M
				SET 
					Value = JSON_REPLACE(Value,  '$.MatchResult', JSON_EXTRACT(matches, CONCAT('$[', i, '].MatchResult'))),
					ModifiedTime = now()
				WHERE M.SportId = sportId
					AND M.Id = @Id
                    AND M.Language = language;
			END IF;
            
        -- Increment the loop variable                                                                                                                                                        
        SET i = i + 1;        
      
    END WHILE;
END
CREATE DEFINER=`user`@`%` PROCEDURE `Score247_InsertOrUpdateMatches`(IN sportId INT, IN matches MEDIUMTEXT, IN language TEXT)
BEGIN
	DECLARE i INT DEFAULT 0;                                                                                                                                                    
    DECLARE e INT DEFAULT JSON_LENGTH(matches);

    WHILE i < e DO                                                                                                              
        INSERT INTO `Match` VALUES (
			JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].Id'))),
			JSON_EXTRACT(matches, CONCAT('$[', i, ']')),
            language, 
            sportId, 
            JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].League.Id'))), 
            STR_TO_DATE(JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].EventDate'))),'%Y-%m-%dT%H:%i:%s+00:00'),
            JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].Region'))),
            now(),
            now())
		ON DUPLICATE KEY UPDATE
            Value = JSON_EXTRACT(matches, CONCAT('$[', i, ']')),
            EventDate = STR_TO_DATE(JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].EventDate'))),'%Y-%m-%dT%H:%i:%s+00:00'),
            ModifiedTime = now();
        -- Increment the loop variable                                                                                                                                                        
        SET i = i + 1;
    END WHILE;
END
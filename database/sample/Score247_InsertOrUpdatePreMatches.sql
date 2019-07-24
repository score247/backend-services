CREATE DEFINER=`user`@`%` PROCEDURE `Score247_InsertOrUpdatePreMatches`(IN matches TEXT)
BEGIN
	DECLARE i INT DEFAULT 0;                                                                                                                                                    
    DECLARE e INT DEFAULT JSON_LENGTH(matches);

    WHILE i < e DO                                                                                                              
        INSERT INTO score247_db2.`Match` VALUES (
			JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].Id'))),
			JSON_EXTRACT(matches, CONCAT('$[', i, '].Value')), 
            JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].Language'))), 
            JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].SportId'))), 
            JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].LeagueId'))), 
            STR_TO_DATE(JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].EventDate'))),'%Y-%m-%dT%H:%i:%s+00:00'),
            JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].Region'))),
            now(),
            now())
            ON DUPLICATE KEY UPDATE
            Value = JSON_EXTRACT(matches, CONCAT('$[', i, '].Value')),
            EventDate = STR_TO_DATE(JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].EventDate'))),'%Y-%m-%dT%H:%i:%s+00:00'),
            ModifiedTime = now();
        -- Increment the loop variable                                                                                                                                                        
        SET i = i + 1;
    END WHILE;
END
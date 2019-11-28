DROP procedure IF EXISTS `League_UpdateCurrentSeason`;

CREATE DEFINER=`user`@`%` PROCEDURE `League_UpdateCurrentSeason`(IN leagues MEDIUMTEXT)
BEGIN
	DECLARE i INT DEFAULT 0;                                                                                                                                                    
    DECLARE e INT DEFAULT JSON_LENGTH(leagues);
    
    WHILE i < e DO
			UPDATE `League` AS League
            SET 
				CurrentSeasonId = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].SeasonId')))
            WHERE League.Id = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Id')))
				AND League.Region = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Region')));
                
			-- Increment the loop variable                                                                                                                                                        
			SET i = i + 1;        
    END WHILE;
END
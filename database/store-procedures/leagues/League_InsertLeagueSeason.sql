DROP procedure IF EXISTS `League_InsertLeagueSeason`;

CREATE DEFINER=`user`@`%` PROCEDURE `League_InsertLeagueSeason`(IN leagues MEDIUMTEXT)
BEGIN
	DECLARE i INT DEFAULT 0;                                                                                                                                                    
    DECLARE e INT DEFAULT JSON_LENGTH(leagues);

    WHILE i < e DO
			INSERT INTO `LeagueSeason`
			(`LeagueId`,
			`SeasonId`,
            `Region`,
            `SeasonDates`,
			`Fetched`,
			`FetchedDate`,
			`CreatedTime`,
			`ModifiedTime`)
				VALUES (
					JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Id'))),
					JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].SeasonId'))),
                    JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Region'))),
                    JSON_EXTRACT(leagues, CONCAT('$[', i, '].SeasonDates')), 
					0,
					NULL,
					NOW(),
					NOW())
                    
				ON DUPLICATE KEY UPDATE
                SeasonDates = JSON_EXTRACT(leagues, CONCAT('$[', i, '].SeasonDates'));
			
			-- Increment the loop variable                                                                                                                                                        
			SET i = i + 1;
    END WHILE;
END
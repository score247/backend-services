DROP procedure IF EXISTS `League_InsertLeagueSeason`;

CREATE DEFINER=`user`@`%` PROCEDURE `League_InsertLeagueSeason`(IN leagues MEDIUMTEXT)
BEGIN
	DECLARE i INT DEFAULT 0;                                                                                                                                                    
    DECLARE e INT DEFAULT JSON_LENGTH(leagues);

    WHILE i < e DO
		IF(NOT EXISTS (SELECT 1 
						FROM  `LeagueSeason` AS S 
                        WHERE S.LeagueId = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Id'))) 
                        AND S.SeasonId = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].SeasonId')))))
		THEN
			INSERT INTO `LeagueSeason`
			(`LeagueId`,
			`SeasonId`,
            `Region`,
			`Fetched`,
			`FetchedDate`,
			`CreatedTime`,
			`ModifiedTime`)
				VALUES (
					JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Id'))),
					JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].SeasonId'))),
                    JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Region'))),
					0,
					NULL,
					NOW(),
					NOW());
			
			-- Increment the loop variable                                                                                                                                                        
			SET i = i + 1;
        END IF;
    END WHILE;
END
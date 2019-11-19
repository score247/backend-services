CREATE DEFINER=`user`@`%` PROCEDURE `League_InsertLeagueSeason`(IN sportId INT, IN leagues MEDIUMTEXT, IN language TEXT)
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
			`Fetched`,
			`FetchedDate`,
			`CreatedTime`,
			`ModifiedTime`)
				VALUES (
					JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Id'))),
					JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].SeasonId'))),
					0,
					NULL,
					NOW(),
					NOW());
			
			-- Increment the loop variable                                                                                                                                                        
			SET i = i + 1;
        END IF;
    END WHILE;
END
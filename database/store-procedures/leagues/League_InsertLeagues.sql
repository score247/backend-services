CREATE DEFINER=`user`@`%` PROCEDURE `League_InsertLeagues`(IN sportId INT, IN leagues MEDIUMTEXT, IN language TEXT)
BEGIN
	DECLARE i INT DEFAULT 0;                                                                                                                                                    
    DECLARE e INT DEFAULT JSON_LENGTH(leagues);

    WHILE i < e DO
		IF (JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].IsInternational'))) = true)
        THEN
			INSERT INTO `League`(`Id`,`Name`, `Region`,`CategoryId`,`IsInternational`, `Language` )
			VALUES (
				JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Id'))),
				JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Name'))),
				JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CategoryId'))),
				JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Region'))),
				'1',
				language)
			ON DUPLICATE KEY UPDATE
				`Name` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Name'))),
				`Region` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Region'))),
				`CategoryId` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CategoryId')));
        ELSE
			INSERT INTO `League`(`Id`,`Name`, `Region`,`CategoryId`, `Country`, `CountryCode`,`IsInternational`, `Language` )
				VALUES (
					JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Id'))),
					JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Name'))),
					JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CategoryId'))),
					JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Region'))),
					JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CountryName'))),
					JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CountryCode'))),
					'0',
					language)
				ON DUPLICATE KEY UPDATE
					`Name` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Name'))),
					`Region` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Region'))),
					`CategoryId` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CategoryId'))),
					`CountryCode` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CountryCode'))),
					`Country` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CountryName')));
        END IF;
        
        -- Increment the loop variable                                                                                                                                                        
        SET i = i + 1;
    END WHILE;
    
    -- Update ContryCode for minor leagues
    SET SQL_SAFE_UPDATES = 0;
	Update `League` Set `CountryCode` = '' WHERE `IsMajor` = 0;
	SET SQL_SAFE_UPDATES = 1;
END
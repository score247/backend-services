DROP procedure IF EXISTS `LiveMatch_UpdateCurrentPeriodStartTime`;

CREATE DEFINER=`user`@`%` PROCEDURE `News_InsertOrUpdateNews`(IN sportId INT, IN news TEXT, IN language VARCHAR(10))
BEGIN
	DECLARE i INT DEFAULT 0;                                                                                                                                                    
    DECLARE e INT DEFAULT JSON_LENGTH(news);
    
    WHILE i < e DO
       INSERT INTO `News`
		(`Title`, `SportId`, `Language`, `Content`, `ImageSource`, `Link`, `PublishedDate`)
		VALUES
			(
			JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].Title'))),
			sportId,
			language,
			JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].Content'))),
			JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].ImageSource'))),
			JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].Link'))),
            JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].PublishedDate'))))
		ON DUPLICATE KEY UPDATE
            Title = JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].Title'))),
            Content = JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].Content'))),
            ImageSource = JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].ImageSource'))),
            PublishedDate = JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].PublishedDate')));
        -- Increment the loop variable                                                                                                                                                        
        SET i = i + 1;
    END WHILE;
END
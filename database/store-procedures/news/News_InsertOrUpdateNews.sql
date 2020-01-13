DROP procedure IF EXISTS `News_InsertOrUpdateNews`;

CREATE DEFINER=`user`@`%` PROCEDURE `News_InsertOrUpdateNews`(IN sportId INT, IN news TEXT, IN language VARCHAR(10))
BEGIN
	DECLARE i INT DEFAULT 0;                                                                                                                                                    
    DECLARE e INT DEFAULT JSON_LENGTH(news);
    
    WHILE i < e DO
       INSERT INTO `News`
		(`Title`, `SportId`, `Language`, `Content`, `ImageSource`, `Link`, `Author`, `PublishedDate`, `CreatedTime`)
		VALUES
			(
			JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].Title'))),
			sportId,
			language,
			JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].Content'))),
			JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].ImageSource'))),
			JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].Link'))),
            JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].Author'))),
            STR_TO_DATE(JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].PublishedDate'))),'%Y-%m-%dT%H:%i:%sZ'),
            now())
		ON DUPLICATE KEY UPDATE
            Title = JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].Title'))),
            Content = JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].Content'))),
            ImageSource = JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].ImageSource'))),
            Author = JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].Author'))),
            PublishedDate = STR_TO_DATE(JSON_UNQUOTE(JSON_EXTRACT(news, CONCAT('$[', i, '].PublishedDate'))),'%Y-%m-%dT%H:%i:%sZ');
        -- Increment the loop variable                                                                                                                                                        
        SET i = i + 1;
    END WHILE;
END
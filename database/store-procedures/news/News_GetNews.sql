DROP procedure IF EXISTS `News_GetNews`;

CREATE DEFINER=`user`@`%` PROCEDURE `News_GetNews`(IN sportId INT, IN language VARCHAR(10))
BEGIN
	SELECT 
		`Id`,
		`Title`,		
		`Content`,
		`ImageSource`,
		`Link`
	FROM `News` AS N
	WHERE N.SportId = sportID
    AND N.Language = language;
END
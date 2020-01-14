DROP procedure IF EXISTS `News_GetImageByName`;

CREATE DEFINER=`user`@`%` PROCEDURE `News_GetImageByName`(IN SportId INT, IN ImageName VARCHAR(250))
BEGIN
	SELECT 
		`Content`
	FROM `NewsImage` AS N
	WHERE N.Name = ImageName 
		AND N.SportId = SportId;

END
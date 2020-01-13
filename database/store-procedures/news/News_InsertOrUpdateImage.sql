DROP procedure IF EXISTS `News_InsertOrUpdateImage`;

CREATE DEFINER=`user`@`%` PROCEDURE `News_InsertOrUpdateImage`(IN sportId INT, IN ImageName VARCHAR(250), IN ImageContent BLOB)
BEGIN
	INSERT INTO `NewsImage`
		(`SportId`,
		`Name`,
		`Content`,
		`CreatedTime`)
	VALUES
		(sportId,
		ImageName,
		ImageContent,
		now())
	on duplicate key update 
		Name = ImageName,
        Content = ImageContent;
END
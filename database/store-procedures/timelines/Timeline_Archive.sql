DROP procedure IF EXISTS `Timeline_Archive`;

CREATE DEFINER=`user`@`%` PROCEDURE `Timeline_Archive`()
BEGIN
	INSERT INTO `Timeline_Archived`
    SELECT * FROM `Timeline` as T
    WHERE T.CreatedTime <= (UTC_TIMESTAMP() - INTERVAL 5 DAY)
    ON DUPLICATE KEY UPDATE
		`Value` = T.`Value`;
    
    DELETE FROM `Timeline`
    WHERE CreatedTime <= (UTC_TIMESTAMP() - INTERVAL 5 DAY);
END
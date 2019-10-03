DROP procedure IF EXISTS `Timeline_Archive`;
CREATE DEFINER=`root`@`%` PROCEDURE `Timeline_Archive`()
BEGIN
	INSERT INTO `Timeline_Archived`
    SELECT * FROM `Timeline` as T
    ON DUPLICATE KEY UPDATE
		`Value` = T.`Value`;
    
    DELETE FROM `Timeline`
    WHERE CreatedTime <= (UTC_TIMESTAMP() - INTERVAL 4 DAY);
END
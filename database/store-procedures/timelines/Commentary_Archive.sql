DROP procedure IF EXISTS `Commentary_Archive`;

CREATE DEFINER=`root`@`%` PROCEDURE `Commentary_Archive`()
BEGIN
	INSERT INTO `Commentary_Archived`
    SELECT * FROM `Commentary` as T
    WHERE T.CreatedTime <= (UTC_TIMESTAMP() - INTERVAL 5 DAY)
    ON DUPLICATE KEY UPDATE
		`Value` = T.`Value`;
    
    DELETE FROM `Commentary`
    WHERE CreatedTime <= (UTC_TIMESTAMP() - INTERVAL 5 DAY);
END
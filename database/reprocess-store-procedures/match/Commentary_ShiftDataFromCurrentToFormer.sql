DROP procedure IF EXISTS `Commentary_ShiftDataFromCurrentToFormer`;

CREATE DEFINER=`user`@`%` PROCEDURE `Commentary_ShiftDataFromCurrentToFormer`()
BEGIN
	START TRANSACTION;
		INSERT INTO score247db_former.`Commentary`
		SELECT * FROM score247db.`Commentary` as T
		WHERE T.EventDate <= (UTC_TIMESTAMP() - INTERVAL 3 DAY)
		ON DUPLICATE KEY UPDATE
			`Value` = T.`Value`;
		
		DELETE FROM score247db.`Commentary`
		WHERE EventDate <= (UTC_TIMESTAMP() - INTERVAL 3 DAY);
	COMMIT;
END
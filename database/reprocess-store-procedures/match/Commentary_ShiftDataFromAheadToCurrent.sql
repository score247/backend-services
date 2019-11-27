DROP procedure IF EXISTS `Commentary_ShiftDataFromAheadToCurrent`;

CREATE DEFINER=`user`@`%` PROCEDURE `Commentary_ShiftDataFromAheadToCurrent`()
BEGIN
	START TRANSACTION;
		INSERT INTO score247db.`Commentary`
		SELECT * FROM score247db_ahead.`Commentary` as T
		WHERE T.EventDate <= (UTC_TIMESTAMP() - INTERVAL 3 DAY)
		ON DUPLICATE KEY UPDATE
			`Value` = T.`Value`;
		
		DELETE FROM score247db_ahead.`Commentary`
		WHERE EventDate <= (UTC_TIMESTAMP() - INTERVAL 3 DAY);
	COMMIT;
END
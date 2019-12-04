DROP procedure IF EXISTS `Lineups_ShiftDataFromAheadToCurrent`;

CREATE DEFINER=`user`@`%` PROCEDURE `Lineups_ShiftDataFromAheadToCurrent`()
BEGIN
	START TRANSACTION;
		INSERT INTO score247db.`Lineups`
		SELECT * FROM score247db_ahead.`Lineups` as T
		WHERE T.EventDate <= (UTC_TIMESTAMP() + INTERVAL 3 DAY)
		ON DUPLICATE KEY UPDATE
			`Value` = T.`Value`;
		
		DELETE FROM score247db_ahead.`Lineups`
		WHERE EventDate <= (UTC_TIMESTAMP() + INTERVAL 3 DAY);
	COMMIT;
END
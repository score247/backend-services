DROP procedure IF EXISTS `Odds_ShiftDataFromAheadToCurrent`;

CREATE DEFINER=`user`@`%` PROCEDURE `Odds_ShiftDataFromAheadToCurrent`()
BEGIN
	START TRANSACTION;
		INSERT INTO score247db.`Odds`
		SELECT * FROM score247db_ahead.`Odds` as T
		WHERE T.EventDate <= (UTC_TIMESTAMP() - INTERVAL 3 DAY)
		ON DUPLICATE KEY UPDATE
			`Value` = T.`Value`;
		
		DELETE FROM score247db_ahead.`Odds`
		WHERE EventDate <= (UTC_TIMESTAMP() - INTERVAL 3 DAY);
	COMMIT;
END
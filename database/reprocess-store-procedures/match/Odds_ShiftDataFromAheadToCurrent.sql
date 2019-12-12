DROP procedure IF EXISTS `Odds_ShiftDataFromAheadToCurrent`;

CREATE DEFINER=`user`@`%` PROCEDURE `Odds_ShiftDataFromAheadToCurrent`()
BEGIN
	
		INSERT INTO score247db.`Odds`
		SELECT * FROM score247db_ahead.`Odds` as T
		WHERE date(T.EventDate) <= (UTC_DATE() + INTERVAL 3 DAY)
		ON DUPLICATE KEY UPDATE
			`Value` = T.`Value`;
		
		DELETE FROM score247db_ahead.`Odds`
		WHERE date(EventDate) <= (UTC_DATE() + INTERVAL 3 DAY);
	
END
DROP procedure IF EXISTS `Odds_ShiftDataFromCurrentToFormer`;

CREATE DEFINER=`user`@`%` PROCEDURE `Odds_ShiftDataFromCurrentToFormer`()
BEGIN
	
		INSERT INTO score247db_former.`Odds`
		SELECT * FROM score247db.`Odds` as T
		WHERE date(T.EventDate) < (UTC_DATE() - INTERVAL 3 DAY)
		ON DUPLICATE KEY UPDATE
			`Value` = T.`Value`;
		
		DELETE FROM score247db.`Odds`
		WHERE date(EventDate) < (UTC_DATE() - INTERVAL 3 DAY);
	
END
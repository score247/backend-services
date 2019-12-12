DROP procedure IF EXISTS `Lineups_ShiftDataFromCurrentToFormer`;

CREATE DEFINER=`user`@`%` PROCEDURE `Lineups_ShiftDataFromCurrentToFormer`()
BEGIN
	
		INSERT INTO score247db_former.`Lineups`
		SELECT * FROM score247db.`Lineups` as T
		WHERE date(T.EventDate) < (UTC_DATE() - INTERVAL 3 DAY)
		ON DUPLICATE KEY UPDATE
			`Value` = T.`Value`;
		
		DELETE FROM score247db.`Lineups`
		WHERE date(EventDate) < (UTC_DATE() - INTERVAL 3 DAY);
	
END
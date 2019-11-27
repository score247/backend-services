DROP procedure IF EXISTS `Odds_ShiftDataFromCurrentToFormer`;

CREATE DEFINER=`user`@`%` PROCEDURE `Odds_ShiftDataFromCurrentToFormer`()
BEGIN
	START TRANSACTION;
		INSERT INTO score247db_former.`Odds`
		SELECT * FROM score247db.`Odds` as T
		WHERE T.EventDate <= (UTC_TIMESTAMP() - INTERVAL 3 DAY)
		ON DUPLICATE KEY UPDATE
			`Value` = T.`Value`;
		
		DELETE FROM score247db.`Odds`
		WHERE EventDate <= (UTC_TIMESTAMP() - INTERVAL 3 DAY);
	COMMIT;
END
DROP procedure IF EXISTS `Match_ShiftDataFromCurrentToFormer`;

CREATE DEFINER=`user`@`%` PROCEDURE `Match_ShiftDataFromCurrentToFormer`()
BEGIN
	
		INSERT INTO score247db_former.Match
		SELECT * FROM score247db.`Match` as M
        WHERE M.EventDate < (UTC_DATE() - INTERVAL 3 DAY)
		ON DUPLICATE KEY UPDATE
			Value = M.`Value`,
			EventDate = M.EventDate;
			
		DELETE FROM score247db.`Match`
		WHERE EventDate < (UTC_DATE() - INTERVAL 3 DAY);
	
END
DROP procedure IF EXISTS `Match_ShiftDataFromAheadToCurrent`;

CREATE DEFINER=`user`@`%` PROCEDURE `Match_ShiftDataFromAheadToCurrent`()
BEGIN
	
		INSERT INTO score247db.Match
		SELECT * FROM score247db_ahead.`Match` as M
        WHERE M.EventDate <= (UTC_TIMESTAMP() + INTERVAL 3 DAY)
		ON DUPLICATE KEY UPDATE
			Value = M.`Value`,
			EventDate = M.EventDate;
			
		DELETE FROM score247db_ahead.`Match`
		WHERE EventDate <= (UTC_TIMESTAMP() + INTERVAL 3 DAY);
	
END
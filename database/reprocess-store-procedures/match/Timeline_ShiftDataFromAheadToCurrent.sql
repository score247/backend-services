DROP procedure IF EXISTS `Timeline_ShiftDataFromAheadToCurrent`;

CREATE DEFINER=`user`@`%` PROCEDURE `Timeline_ShiftDataFromAheadToCurrent`()
BEGIN
	INSERT INTO score247db.`Timeline`
    SELECT * FROM score247db_ahead.`Timeline` as T
    WHERE T.CreatedTime <= (UTC_TIMESTAMP() - INTERVAL 3 DAY)
    ON DUPLICATE KEY UPDATE
		`Value` = T.`Value`;
    
    DELETE FROM score247db_ahead.`Timeline`
    WHERE CreatedTime <= (UTC_TIMESTAMP() - INTERVAL 3 DAY);
END
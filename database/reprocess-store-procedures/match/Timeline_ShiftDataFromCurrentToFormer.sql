DROP procedure IF EXISTS `Timeline_ShiftDataFromCurrentToFormer`;

CREATE DEFINER=`user`@`%` PROCEDURE `Timeline_ShiftDataFromCurrentToFormer`()
BEGIN

	INSERT INTO score247db_former.`Timeline`
    SELECT * FROM score247db.`Timeline` as T
    WHERE T.CreatedTime <= (UTC_TIMESTAMP() - INTERVAL 3 DAY)
    ON DUPLICATE KEY UPDATE
		`Value` = T.`Value`;
    
    DELETE FROM score247db.`Timeline`
    WHERE CreatedTime <= (UTC_TIMESTAMP() - INTERVAL 3 DAY);
END
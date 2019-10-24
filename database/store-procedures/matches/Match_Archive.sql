DROP procedure IF EXISTS `Match_Archive`;

CREATE DEFINER=`user`@`%` PROCEDURE `Match_Archive`()
BEGIN
	INSERT INTO Match_Archived
		SELECT * FROM `Match` as M
        WHERE M.EventDate <= (UTC_TIMESTAMP() - INTERVAL 5 DAY)
		ON DUPLICATE KEY UPDATE
			Value = M.`Value`,
			EventDate = M.EventDate;
			
		DELETE FROM `Match`
		WHERE EventDate <= (UTC_TIMESTAMP() - INTERVAL 5 DAY);
END
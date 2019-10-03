DROP procedure IF EXISTS `Match_Archive`;

CREATE DEFINER=`root`@`%` PROCEDURE `Match_Archive`()
BEGIN
	INSERT INTO Match_Archived
    SELECT * FROM `Match` as M
    ON DUPLICATE KEY UPDATE
		Value = M.`Value`,
		EventDate = M.EventDate;
        
	DELETE FROM `Match`
    WHERE EventDate BETWEEN (UTC_TIMESTAMP() - INTERVAL 4 DAY) AND (UTC_TIMESTAMP() + INTERVAL 4 DAY);
END
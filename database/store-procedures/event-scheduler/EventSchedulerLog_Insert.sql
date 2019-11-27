DROP procedure IF EXISTS `EventSchedulerLog_Insert`;

CREATE DEFINER=`user`@`%` PROCEDURE `EventSchedulerLog_Insert`(IN eventName VARCHAR(100), IN status VARCHAR(100))
BEGIN
	INSERT INTO `EventSchedulerLog`
		(`EventName`,
		`Status`)
	VALUES(
		eventName,
        status);
END
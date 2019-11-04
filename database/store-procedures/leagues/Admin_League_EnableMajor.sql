DROP procedure IF EXISTS `Admin_League_EnableMajor`;

CREATE DEFINER=`user`@`%` PROCEDURE `Admin_League_EnableMajor`()
BEGIN
	SET SQL_SAFE_UPDATES = 0;
	UPDATE `League` SET `IsActive` = `IsMajor`;
    SET SQL_SAFE_UPDATES = 1;
END
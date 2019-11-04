DROP procedure IF EXISTS `Admin_League_EnableAll`;

CREATE DEFINER=`user`@`%` PROCEDURE `Admin_League_EnableAll`()
BEGIN
	SET SQL_SAFE_UPDATES = 0;
	UPDATE `League` SET `IsActive` = '1';
    SET SQL_SAFE_UPDATES = 1;
END
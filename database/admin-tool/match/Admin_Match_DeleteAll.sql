DROP procedure IF EXISTS `Admin_Match_DeleteAll`;

CREATE DEFINER=`user`@`%` PROCEDURE `Admin_Match_DeleteAll`()
BEGIN
	SET SQL_SAFE_UPDATES=0;
    
	TRUNCATE TABLE `Lineups`;
	TRUNCATE TABLE `HeadToHead`;
	TRUNCATE TABLE `Commentary`;
    TRUNCATE TABLE `Timeline`;
	TRUNCATE TABLE `LiveMatch`;
	TRUNCATE TABLE `Match`;
    
	SET SQL_SAFE_UPDATES=1;
END
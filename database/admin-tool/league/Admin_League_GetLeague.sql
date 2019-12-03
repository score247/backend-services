DROP procedure IF EXISTS `Admin_League_GetLeague`;

CREATE DEFINER=`user`@`%` PROCEDURE `Admin_League_GetLeague`()
BEGIN
	SELECT * FROM League as lg Order By lg.Order;
END
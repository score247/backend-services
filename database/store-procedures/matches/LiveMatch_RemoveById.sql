CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_RemoveById`(IN matchId varchar(50))
BEGIN
	DELETE FROM `LiveMatch` WHERE Id = matchId;
END
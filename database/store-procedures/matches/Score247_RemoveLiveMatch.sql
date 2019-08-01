CREATE DEFINER=`root`@`%` PROCEDURE `Score247_RemoveLiveMatch`(IN matchId varchar(50))
BEGIN
	DELETE FROM `LiveMatch` WHERE Id = matchId;
END
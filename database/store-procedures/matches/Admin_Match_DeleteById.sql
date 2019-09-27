DROP procedure IF EXISTS `Admin_Match_DeleteById`;

CREATE DEFINER=`user`@`%` PROCEDURE `Admin_Match_DeleteById`(IN id varchar(45))
BEGIN
	SET SQL_SAFE_UPDATES=0;
    DELETE FROM `Timeline` as TL WHERE TL.MatchId = id;
	DELETE FROM `LiveMatch` as LM WHERE LM.Id = id;
	DELETE FROM `Match` as MC WHERE MC.Id = id;
	SET SQL_SAFE_UPDATES=1;
END
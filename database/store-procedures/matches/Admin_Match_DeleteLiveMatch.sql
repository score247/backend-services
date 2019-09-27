DROP procedure IF EXISTS `Admin_Match_DeleteLiveMatch`;

CREATE DEFINER=`user`@`%` PROCEDURE `Admin_Match_DeleteLiveMatch`(IN limitCount INT)
BEGIN
	SET SQL_SAFE_UPDATES=0;
    DROP TEMPORARY TABLE IF EXISTS LiveMatchIds;
    CREATE TEMPORARY TABLE LiveMatchIds SELECT Id
		FROM `LiveMatch`
        LIMIT limitCount;
        
    DELETE FROM `Timeline` WHERE `MatchId` IN (SELECT Id from LiveMatchIds);
	DELETE FROM `LiveMatch` where Id IN (SELECT Id from LiveMatchIds);
	DELETE FROM `Match` where Id IN (SELECT Id from LiveMatchIds);
    
	DROP TEMPORARY TABLE IF EXISTS LiveMatchIds;
	SET SQL_SAFE_UPDATES=1;
END
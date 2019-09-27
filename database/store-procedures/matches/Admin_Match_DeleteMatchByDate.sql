DROP procedure IF EXISTS `Admin_Match_DeleteMatchByDate`;

CREATE DEFINER=`user`@`%` PROCEDURE `Admin_Match_DeleteMatchByDate`(IN fromDate DATETIME, IN toDate DATETIME, IN limitCount INT)
BEGIN
	SET SQL_SAFE_UPDATES=0;
    DROP TEMPORARY TABLE IF EXISTS matchIds;
    CREATE TEMPORARY TABLE matchIds SELECT Id
		FROM 
			(SELECT Id, EventDate FROM `Match` as NonLiveMatch  WHERE Id NOT IN (SELECT  Id FROM    LiveMatch as LM)
			UNION ALL (SELECT Id, EventDate FROM `LiveMatch` as LM)) as AllMatches
		WHERE EventDate >= fromDate and EventDate <= toDate
        LIMIT limitCount;
        
    DELETE FROM `Timeline` WHERE `MatchId` IN (SELECT Id from matchIds);
	DELETE FROM `Match` where Id IN (SELECT Id from matchIds);
	DELETE FROM `LiveMatch` where Id IN (SELECT Id from matchIds);
    
	DROP TEMPORARY TABLE IF EXISTS matchIds;
	SET SQL_SAFE_UPDATES=1;
END
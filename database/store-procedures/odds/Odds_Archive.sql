DROP procedure IF EXISTS `Odds_Archive`;

CREATE DEFINER=`user`@`%` PROCEDURE `Odds_Archive`()
BEGIN
	INSERT INTO Odds_Archived
						(`CreatedTime`,
						`Value`,
						`MatchId`,
						`BetTypeId`,
						`BookmakerId`)
    SELECT o.`CreatedTime`, o.`Value`, o.`matchId`, o.`BetTypeId`, o.`BookmakerId` FROM `Odds` AS o
    WHERE o.CreatedTime <= (UTC_TIMESTAMP() - INTERVAL 10 DAY);
        
	DELETE FROM `Odds` AS o
    WHERE o.CreatedTime <= (UTC_TIMESTAMP() - INTERVAL 10 DAY);
END
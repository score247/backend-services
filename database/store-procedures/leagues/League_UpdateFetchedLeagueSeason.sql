DROP procedure IF EXISTS `League_UpdateFetchedLeagueSeason`;

CREATE DEFINER=`user`@`%` PROCEDURE `League_UpdateFetchedLeagueSeason`(IN leagueSeasons MEDIUMTEXT)
BEGIN
	DECLARE i INT DEFAULT 0;                                                                                                                                                    
    DECLARE e INT DEFAULT JSON_LENGTH(leagueSeasons);
    
    WHILE i < e DO
			UPDATE `LeagueSeason` AS LS
            SET 
				Fetched = 1,
				ModifiedTime = now(),
				FetchedDate = now()
            WHERE LS.LeagueId = JSON_UNQUOTE(JSON_EXTRACT(leagueSeasons, CONCAT('$[', i, '].LeagueId')))
				AND LS.SeasonId = JSON_UNQUOTE(JSON_EXTRACT(leagueSeasons, CONCAT('$[', i, '].SeasonId')))
				AND LS.Region = JSON_UNQUOTE(JSON_EXTRACT(leagueSeasons, CONCAT('$[', i, '].Region')));
            
			-- Increment the loop variable                                                                                                                                                        
			SET i = i + 1;        
    END WHILE;
END
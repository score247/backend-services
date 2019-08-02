 DELIMITER //
CREATE DEFINER=`user`@`%` PROCEDURE `Score247_Odds_InsertOdds`(
		IN MatchId VARCHAR(45), 
        IN OddsList TEXT)
	BEGIN
		DECLARE oddsIndex INT DEFAULT 0;
		DECLARE totalOdds INT DEFAULT JSON_LENGTH(matches);
		
		WHILE oddsIndex < totalOdds DO                                                                                                              
			INSERT INTO score247_db2.`Odds` VALUES (
				CreatedTime = NOW();
			-- Increment the loop variable                                                                                                                                                        
			SET oddsIndex = oddsIndex + 1;
		END WHILE;
	END
DELIMITER ;
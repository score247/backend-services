DELIMITER | 
CREATE EVENT Event_ShiftMatchesToNextDay
    ON SCHEDULE
      EVERY 1 DAY 
      STARTS '2019-09-03 23:00:00'
    DO  	        
    BEGIN
      UPDATE `Match` 
      SET 
		EventDate = DATE_ADD(EventDate, INTERVAL 1 DAY),
        `Value` = JSON_REPLACE(`Value`,  '$.EventDate', DATE_FORMAT(EventDate, "%Y-%m-%dT%H:%i:%s+00:00")),
		ModifiedTime = NOW();   
      
      UPDATE LiveMatch 
      SET 
		EventDate = DATE_ADD(EventDate, INTERVAL 1 DAY),
        `Value` = JSON_REPLACE(`Value`,  '$.EventDate', DATE_FORMAT(EventDate, "%Y-%m-%dT%H:%i:%s+00:00")),
		ModifiedTime = NOW();
        
		UPDATE Timeline
        SET 
         `Value` = JSON_REPLACE(`Value`,  '$.Time', DATE_FORMAT(DATE_ADD(STR_TO_DATE(Value->>"$.Time", "%Y-%m-%dT%H:%i:%s+00:00"), INTERVAL 1 DAY), "%Y-%m-%dT%H:%i:%s+00:00") ),
        ModifiedTime = NOW();
        
        UPDATE Odds
        SET 
         `Value` = JSON_REPLACE(`Value`,  '$.LastUpdatedTime', DATE_FORMAT(DATE_ADD(STR_TO_DATE(Value->>"$.LastUpdatedTime", "%Y-%m-%dT%H:%i:%sZ"), INTERVAL 1 DAY), "%Y-%m-%dT%H:%i:%s+00:00") ),
        CreatedTime =  DATE_ADD(CreatedTime, INTERVAL 1 DAY);       
	END
    
    | DELIMITER ;

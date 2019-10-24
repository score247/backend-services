#http://www.mysqltutorial.org/mysql-triggers/working-mysql-scheduled-event/
SHOW EVENTS;
DROP EVENT IF EXISTS Event_ArchiveData;
DELIMITER | 
CREATE EVENT Event_ArchiveData
    ON SCHEDULE
      EVERY 3 HOUR
      STARTS CURRENT_TIMESTAMP
    DO  
    BEGIN 
		CALL Match_Archive();
        CALL Timeline_Archive();
        CALL Odds_Archive();
		CALL Commentary_Archive();
    END
    | DELIMITER ;
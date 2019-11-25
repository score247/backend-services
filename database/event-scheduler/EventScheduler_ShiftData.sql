DROP EVENT IF EXISTS Event_ShiftMatchData;

CREATE EVENT Event_ShiftMatchData
    ON SCHEDULE
      EVERY 5 MINUTE
      STARTS CURRENT_TIMESTAMP
    DO  
    BEGIN 
		Call Match_ShiftDataFromCurrentToFormer();
		Call Match_ShiftDataFromAheadToCurrent();
		
		Call Timeline_ShiftDataFromCurrentToFormer();
		Call Timeline_ShiftDataFromAheadToCurrent();
		
		Call Commentary_ShiftDataFromCurrentToFormer();
		Call Commentary_ShiftDataFromAheadToCurrent();
    END
 
DROP EVENT IF EXISTS Event_ShiftMatchData_environment;

CREATE EVENT Event_ShiftMatchData_environment
    ON SCHEDULE
      EVERY 6 HOUR
      STARTS CURRENT_TIMESTAMP
    DO  
    BEGIN 
		Call score247db.EventSchedulerLog_Insert('Event_ShiftMatchData_environment', 'start');
		
		Call score247db.Match_ShiftDataFromCurrentToFormer();
		Call score247db.Match_ShiftDataFromAheadToCurrent();
		
		Call score247db.Timeline_ShiftDataFromCurrentToFormer();
		
		Call score247db.Commentary_ShiftDataFromCurrentToFormer();
		
		Call score247db.Lineups_ShiftDataFromAheadToCurrent();
		Call score247db.Lineups_ShiftDataFromCurrentToFormer();
		
		Call score247db.Odds_ShiftDataFromAheadToCurrent();
		Call score247db.Odds_ShiftDataFromCurrentToFormer();
		
		Call score247db.EventSchedulerLog_Insert('Event_ShiftMatchData_environment', 'end');
    END
 
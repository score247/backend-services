DROP EVENT IF EXISTS Event_DeleteNews;

CREATE EVENT Event_DeleteNews
    ON SCHEDULE
      EVERY 24 HOUR
      STARTS '2020-01-14 00:01:00'
    DO  
    BEGIN 
		Call EventSchedulerLog_Insert('Event_DeleteNews', 'start');
		
		DELETE FROM `News`
		WHERE date(PublishedDate) < (UTC_DATE() - INTERVAL 7 DAY);
		
		DELETE FROM `NewsImage`
		WHERE date(CreatedTime) < (UTC_DATE() - INTERVAL 7 DAY);
		
		Call EventSchedulerLog_Insert('Event_DeleteNews', 'end');
    END
 
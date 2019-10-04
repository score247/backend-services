SET SQL_SAFE_UPDATES = 0;
Update `League` Set `IsMajor` = `IsActive`;
Update `League` Set `IsActive` = '1';
SET SQL_SAFE_UPDATES = 1;
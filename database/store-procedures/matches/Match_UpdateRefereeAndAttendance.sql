CREATE DEFINER=`user`@`%` PROCEDURE `Match_UpdateRefereeAndAttendance`(
	IN sportId INT, 
    IN matchId VARCHAR(45), 
    IN referee VARCHAR(200),
    IN attendance INT,
    IN language VARCHAR(10))
BEGIN
	UPDATE `Match` as M
    SET 
		Value = JSON_REPLACE(Value,  '$.Referee', referee, '$.Attendance', attendance),
        ModifiedTime = now()
	WHERE M.SportId = sportId
		AND M.Id = matchId
        AND M.Language = language;
END
DROP procedure IF EXISTS `Match_GetByIds`;

CREATE DEFINER=`user`@`%` PROCEDURE `Match_GetByIds`(IN Ids TEXT, IN language VARCHAR(10))
BEGIN
	DECLARE i INT DEFAULT 0;    
	DECLARE count INT DEFAULT JSON_LENGTH(Ids);
    DECLARE IdStr TINYTEXT DEFAULT '';
    
    CREATE TEMPORARY TABLE ids_tbl (
		Id VARCHAR(45)
    );
    
     WHILE i < count DO    
		INSERT INTO ids_tbl(`Id`) VALUES ( JSON_UNQUOTE(JSON_EXTRACT(Ids, CONCAT('$[', i, '].MatchId'))));
		SET i = i + 1;
     END WHILE;
     
     SELECT JSON_REPLACE(M.Value,  '$.ModifiedTime', M.ModifiedTime) AS Value 
     FROM `Match` AS M INNER JOIN ids_tbl AS temp ON M.Id = temp.Id;
     
     DROP TEMPORARY TABLE ids_tbl;
END
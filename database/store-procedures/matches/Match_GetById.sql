CREATE DEFINER=`user`@`%` PROCEDURE `Match_GetById`(IN Id VARCHAR(45), IN language TEXT)
BEGIN
	IF EXISTS (SELECT 1 FROM `LiveMatch` as LM WHERE LM.Id = Id AND LM.`Language` = language) 
	THEN
		SELECT `Value` FROM `LiveMatch` as LM  WHERE LM.Id = Id AND LM.`Language` = language;
	ELSE
		SELECT `Value` FROM `Match` as M  WHERE M.Id = Id AND M.`Language` = language;
	END IF;
END
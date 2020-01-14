DROP procedure IF EXISTS `League_EnableHasGroups`;

CREATE PROCEDURE `League_EnableHasGroups` (IN LeagueId TINYTEXT)
BEGIN
	UPDATE League as L
    SET L.HasGroups = true
    WHERE L.Id = LeagueId;
END
DROP procedure IF EXISTS `League_EnableHasGroups`;

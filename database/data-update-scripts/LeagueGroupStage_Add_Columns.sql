ALTER TABLE `LeagueGroupStage` 
ADD COLUMN `HasStanding` TINYINT NULL DEFAULT '0' AFTER `ModifiedTime`;
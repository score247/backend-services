
ALTER TABLE `League` 
ADD COLUMN `Abbreviation` VARCHAR(30) NULL DEFAULT '' AFTER `Country`;

ALTER TABLE `LeagueGroupStage` 
ADD COLUMN `GroupName` VARCHAR(250) NULL DEFAULT '' AFTER `GroupStageName`;
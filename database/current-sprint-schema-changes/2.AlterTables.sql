alter table `Match` 
add column `LeagueSeasonId` varchar(45) null default null after `Region`;

alter table `LiveMatch` 
add column `LeagueSeasonId` varchar(45) null default null after `Region`;

alter table `Commentary` 
add column `EventDate` timestamp null after `Id`;

alter table `Timeline` 
add column `EventDate` timestamp null after `ModifiedTime`;

alter table `Lineups` 
add column `EventDate` timestamp null after `ModifiedTime`;

ALTER TABLE `Odds` 
ADD COLUMN `EventDate` TIMESTAMP NULL AFTER `Id`;

ALTER TABLE `Timeline` 
ADD COLUMN `Type` VARCHAR(45) NULL AFTER `EventDate`;

ALTER TABLE `score247_local_dev`.`League` 
ADD COLUMN `CurrentSeasonId` VARCHAR(45) NULL AFTER `IsInternational`;
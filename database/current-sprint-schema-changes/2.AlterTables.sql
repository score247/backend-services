
ALTER TABLE `League` 
ADD COLUMN `Abbreviation` VARCHAR(30) NULL DEFAULT '' AFTER `Country`;

ALTER TABLE `score247_local_dev`.`Timeline` 
ADD COLUMN `Language` VARCHAR(10) NOT NULL DEFAULT 'en-US' AFTER `Value`,
DROP PRIMARY KEY,
ADD PRIMARY KEY (`Id`, `MatchId`, `Language`);
;
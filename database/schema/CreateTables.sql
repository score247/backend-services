CREATE TABLE `Match` (
  `Id` varchar(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` json NOT NULL,
  `Language` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `SportId` int(11) NOT NULL,
  `LeagueId` varchar(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `EventDate` timestamp NOT NULL,
  `Region` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `CreatedTime` timestamp NULL DEFAULT NULL,
  `ModifiedTime` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`Id`,`Language`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `LiveMatch` (
  `Id` varchar(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` json NOT NULL,
  `Language` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `SportId` int(11) NOT NULL,
  `LeagueId` varchar(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `EventDate` timestamp NOT NULL,
  `Region` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `CreatedTime` timestamp NULL DEFAULT NULL,
  `ModifiedTime` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`Id`,`Language`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Timeline` (
  `Id` bigint(20) NOT NULL,
  `MatchId` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` json NOT NULL,
  `CreatedTime` timestamp NULL DEFAULT NULL,
  `ModifiedTime` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`Id`,`MatchId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `Odds` (
    `Id` DOUBLE AUTO_INCREMENT,
    `CreatedTime` TIMESTAMP NOT NULL,
    `Value` JSON NOT NULL,
    `MatchId` VARCHAR(45) COLLATE utf8mb4_unicode_ci NOT NULL,
    `BetTypeId` INTEGER NOT NULL,
    `BookmakerId` VARCHAR(20) COLLATE utf8mb4_unicode_ci NOT NULL,
    CONSTRAINT `PK_Odds` PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `TeamStatistic` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Value` JSON NOT NULL,
  `SportId` INT NOT NULL,
  `MatchId` VARCHAR(45) NOT NULL,
  `TeamId` VARCHAR(45) NOT NULL,
  `LeagueId` VARCHAR(45) NOT NULL,
  `Region` VARCHAR(10) NOT NULL,
  `CreatedTime` TIMESTAMP NOT NULL,
  `ModifiedTime` TIMESTAMP NOT NULL,
  PRIMARY KEY (`Id`));


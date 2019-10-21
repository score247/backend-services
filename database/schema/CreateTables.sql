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
  `Language` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'en-US',
  `CreatedTime` timestamp NULL DEFAULT NULL,
  `ModifiedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
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
 
 CREATE TABLE `League` (
  `Id` VARCHAR(45) NOT NULL,
  `Name` VARCHAR(255) NULL,
  `Order` INT NULL DEFAULT 10000,
  `CategoryId` VARCHAR(45) NULL,
  `Country` VARCHAR(255) NULL,
  `Region` VARCHAR(45) NULL,
  `IsActive` TINYINT NULL DEFAULT '0',
  `CountryCode` VARCHAR(45) NULL,
  `IsMajor` TINYINT NULL DEFAULT '0',
  `IsInternational` TINYINT NULL DEFAULT '0',
  PRIMARY KEY (`Id`, `Language`));
 
CREATE TABLE IF NOT EXISTS `Match_Archived` (
  `Id` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` json NOT NULL,
  `Language` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `SportId` INT NOT NULL,
  `LeagueId` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `EventDate` timestamp NOT NULL,
  `Region` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `CreatedTime` timestamp NULL DEFAULT NULL,
  `ModifiedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`,`Language`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `Timeline_Archived` (
  `Id` BIGINT NOT NULL,
  `MatchId` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` json NOT NULL,
  `Language` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'en-US',
  `CreatedTime` timestamp NULL DEFAULT NULL,
  `ModifiedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`,`MatchId`,`Language`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `Odds_Archived` (
    `Id` INT AUTO_INCREMENT NOT NULL,
    `CreatedTime` TIMESTAMP NOT NULL,
    `Value` JSON NOT NULL,
    `MatchId` VARCHAR(45) COLLATE utf8mb4_unicode_ci NOT NULL,
    `BetTypeId` INT NOT NULL,
    `BookmakerId` VARCHAR(20) COLLATE utf8mb4_unicode_ci NOT NULL,
    CONSTRAINT `PK_Odds` PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Commentary` (
  `TimelineId` bigint(20) NOT NULL,
  `MatchId` varchar(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` json NOT NULL,
  `Language` varchar(10) NOT NULL,
  `CreatedTime` timestamp NOT NULL,
  `ModifiedTime` timestamp NOT NULL,
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;





 


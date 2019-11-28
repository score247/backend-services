DROP TABLE IF EXISTS `LeagueSeason`;
CREATE TABLE `LeagueSeason` (
  `LeagueId` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `SeasonId` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Region` varchar(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Fetched` tinyint(4) DEFAULT '0',
  `FetchedDate` timestamp NULL DEFAULT NULL,
  `CreatedTime` timestamp NULL DEFAULT NULL,
  `ModifiedTime` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`SeasonId`,`LeagueId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

DROP TABLE IF EXISTS `Standings`;
CREATE TABLE `Standings` (
  `LeagueId` VARCHAR(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `SeasonId` VARCHAR(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Language` VARCHAR(10) NOT NULL,
  `TableType` VARCHAR(10) NOT NULL,
  `Value` JSON NOT NULL,
  `CreatedTime` TIMESTAMP NOT NULL,
  `ModifiedTime` TIMESTAMP NOT NULL,
  PRIMARY KEY (`LeagueId`, `SeasonId`, `Language`, `TableType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

DROP TABLE IF EXISTS `EventSchedulerLog`;
CREATE TABLE `EventSchedulerLog` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `EventName` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Status` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=810 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
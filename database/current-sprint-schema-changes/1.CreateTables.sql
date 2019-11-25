CREATE TABLE IF NOT EXISTS `LeagueSeason` (
  `LeagueId` varchar(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `SeasonId` varchar(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Region` VARCHAR(45) NOT NULL,
  `Fetched` TINYINT NULL DEFAULT '0',
  `FetchedDate` timestamp NULL DEFAULT NULL,
  `CreatedTime` timestamp NULL DEFAULT NULL,
  `ModifiedTime` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`SeasonId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `Standings` (
  `LeagueId` VARCHAR(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `SeasonId` VARCHAR(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Language` VARCHAR(10) NOT NULL,
  `TableType` VARCHAR(10) NOT NULL,
  `Value` JSON NOT NULL,
  `CreatedTime` TIMESTAMP NOT NULL,
  `ModifiedTime` TIMESTAMP NOT NULL,
  PRIMARY KEY (`LeagueId`, `SeasonId`, `Language`, `TableType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
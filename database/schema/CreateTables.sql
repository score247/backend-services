CREATE TABLE `Match` (
  `Id` varchar(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` json NOT NULL,
  `Language` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `SportId` int(11) NOT NULL,
  `LeagueId` varchar(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `EventDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
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
  `EventDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Region` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `CreatedTime` timestamp NULL DEFAULT NULL,
  `ModifiedTime` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`Id`,`Language`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


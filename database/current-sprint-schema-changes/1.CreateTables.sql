DROP TABLE IF EXISTS `LeagueGroupStage`;

CREATE TABLE `LeagueGroupStage` (
  `LeagueId` varchar(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `LeagueSeasonId` varchar(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `GroupStageName` varchar(250) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` json DEFAULT NULL,
  `Language` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `CreatedTime` timestamp NOT NULL,
  `ModifiedTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`LeagueId`,`GroupStageName`,`LeagueSeasonId`,`Language`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
DROP TABLE IF EXISTS `News`;
CREATE TABLE `News` (
  `Title` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `SportId` int(11) NOT NULL,
  `Language` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Content` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ImageSource` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Link` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Author` varchar(250) COLLATE utf8mb4_unicode_ci NOT NULL,
  `PublishedDate` timestamp NOT NULL,
  `CreatedTime` timestamp NOT NULL,
  `ModifiedTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Language`,`Link`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

DROP TABLE IF EXISTS `NewsImage`;
CREATE TABLE `NewsImage` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `SportId` int(11) NOT NULL,
  `Name` varchar(250) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Content` mediumblob NOT NULL,
  `CreatedTime` timestamp NOT NULL,
  `ModifiedTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

DROP TABLE IF EXISTS `News`;
CREATE TABLE `News` (  
  `Title` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL,
  `SportId` int(11) NOT NULL,
  `Language` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Content` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `ImageSource` varchar(250) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Link` varchar(250) COLLATE utf8mb4_unicode_ci NOT NULL,
  `PublishedDate` timestamp NOT NULL,
  `CreatedTime` timestamp NOT NULL,
  `ModifiedTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Link`,`Language`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
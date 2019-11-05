-- MySQL dump 10.13  Distrib 8.0.16, for Win64 (x86_64)
--
-- Host: 10.18.200.109    Database: score247_local_main
-- ------------------------------------------------------
-- Server version	8.0.17

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
 SET NAMES utf8 ;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `Commentary`
--

DROP TABLE IF EXISTS `Commentary`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `Commentary` (
  `TimelineId` bigint(20) NOT NULL,
  `MatchId` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` json NOT NULL,
  `Language` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `CreatedTime` timestamp NOT NULL,
  `ModifiedTime` timestamp NOT NULL,
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`Id`),
  KEY `commentary_match_index` (`MatchId`)
) ENGINE=InnoDB AUTO_INCREMENT=168671 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Commentary_Archived`
--

DROP TABLE IF EXISTS `Commentary_Archived`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `Commentary_Archived` (
  `TimelineId` bigint(20) NOT NULL,
  `MatchId` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` json NOT NULL,
  `Language` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `CreatedTime` timestamp NOT NULL,
  `ModifiedTime` timestamp NOT NULL,
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=130506 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `HeadToHead`
--

DROP TABLE IF EXISTS `HeadToHead`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `HeadToHead` (
  `HomeTeamId` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `AwayTeamId` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `MatchId` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Language` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'en-US',
  `Value` json NOT NULL,
  `CreatedTime` timestamp NULL DEFAULT NULL,
  `ModifiedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`HomeTeamId`,`AwayTeamId`,`MatchId`,`Language`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `League`
--

DROP TABLE IF EXISTS `League`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `League` (
  `Id` varchar(45) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Name` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Order` int(11) DEFAULT '10000',
  `CategoryId` varchar(45) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Country` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Region` varchar(45) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `IsActive` tinyint(4) DEFAULT '0',
  `CountryCode` varchar(45) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Language` varchar(45) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'en-US',
  `IsMajor` tinyint(4) DEFAULT '0',
  `IsInternational` tinyint(4) DEFAULT '0',
  PRIMARY KEY (`Id`,`Language`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Lineups`
--

DROP TABLE IF EXISTS `Lineups`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `Lineups` (
  `MatchId` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` json NOT NULL,
  `Language` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `CreatedTime` timestamp NOT NULL,
  `ModifiedTime` timestamp NOT NULL,
  PRIMARY KEY (`MatchId`,`Language`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Lineups_Archived`
--

DROP TABLE IF EXISTS `Lineups_Archived`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `Lineups_Archived` (
  `MatchId` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` json NOT NULL,
  `Language` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `CreatedTime` timestamp NOT NULL,
  `ModifiedTime` timestamp NOT NULL,
  PRIMARY KEY (`MatchId`,`Language`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `LiveMatch`
--

DROP TABLE IF EXISTS `LiveMatch`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `LiveMatch` (
  `Id` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` json NOT NULL,
  `Language` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `SportId` int(11) NOT NULL,
  `LeagueId` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `EventDate` timestamp NOT NULL,
  `Region` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `CreatedTime` timestamp NULL DEFAULT NULL,
  `ModifiedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`,`Language`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Match`
--

DROP TABLE IF EXISTS `Match`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `Match` (
  `Id` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` json NOT NULL,
  `Language` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `SportId` int(11) NOT NULL,
  `LeagueId` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `EventDate` timestamp NOT NULL,
  `Region` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `CreatedTime` timestamp NULL DEFAULT NULL,
  `ModifiedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`,`Language`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Match_Archived`
--

DROP TABLE IF EXISTS `Match_Archived`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `Match_Archived` (
  `Id` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` json NOT NULL,
  `Language` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `SportId` int(11) NOT NULL,
  `LeagueId` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `EventDate` timestamp NOT NULL,
  `Region` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `CreatedTime` timestamp NULL DEFAULT NULL,
  `ModifiedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`,`Language`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Odds`
--

DROP TABLE IF EXISTS `Odds`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `Odds` (
  `CreatedTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Value` json NOT NULL,
  `MatchId` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `BetTypeId` int(11) NOT NULL,
  `BookmakerId` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1503175 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Odds_Archived`
--

DROP TABLE IF EXISTS `Odds_Archived`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `Odds_Archived` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedTime` timestamp NOT NULL,
  `Value` json NOT NULL,
  `MatchId` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `BetTypeId` int(11) NOT NULL,
  `BookmakerId` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1025728 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Timeline`
--

DROP TABLE IF EXISTS `Timeline`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `Timeline` (
  `Id` bigint(20) NOT NULL,
  `MatchId` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` json NOT NULL,
  `Language` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'en-US',
  `CreatedTime` timestamp NULL DEFAULT NULL,
  `ModifiedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`,`MatchId`,`Language`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Timeline_Archived`
--

DROP TABLE IF EXISTS `Timeline_Archived`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `Timeline_Archived` (
  `Id` bigint(20) NOT NULL,
  `MatchId` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` json NOT NULL,
  `Language` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'en-US',
  `CreatedTime` timestamp NULL DEFAULT NULL,
  `ModifiedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`,`MatchId`,`Language`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping routines for database 'score247_local_main'
--
/*!50003 DROP PROCEDURE IF EXISTS `Admin_League_EnableAll` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Admin_League_EnableAll`()
BEGIN
	SET SQL_SAFE_UPDATES = 0;
	UPDATE `League` SET `IsActive` = '1';
    SET SQL_SAFE_UPDATES = 1;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Admin_League_EnableMajor` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Admin_League_EnableMajor`()
BEGIN
	SET SQL_SAFE_UPDATES = 0;
	UPDATE `League` SET `IsActive` = `IsMajor`;
    SET SQL_SAFE_UPDATES = 1;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Admin_League_GetLeague` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Admin_League_GetLeague`()
BEGIN
	SELECT * FROM League as lg Order By lg.Order;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Admin_League_InsertLeague` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Admin_League_InsertLeague`(
		IN leagueId VARCHAR(45), 
        IN leagueName varchar(255),
        IN leagueOrder Int,
        IN categoryId varchar(45),
        IN country varchar(255),
        IN countryCode varchar(45),
        IN region varchar(45),
        IN isActive tinyint,
        IN language varchar(45))
BEGIN
if not exists (select * from `League` where `Id` = leagueId) then
	INSERT INTO `League`
		(`Id`, `Name`, `Order`, `CategoryId`, `Country`, `CountryCode`,
        `Region`, `IsActive`, `Language`)
        VALUE (leagueId, leagueName, leagueOrder, categoryId, country, countryCode,
        region, isActive, language);
end if;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Admin_League_UpdateActiveStatus` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Admin_League_UpdateActiveStatus`(IN leagueId varchar(45), IN isActive tinyint)
BEGIN
	UPDATE `League` SET `IsActive` = isActive WHERE `Id` = leagueId;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Admin_Match_DeleteAll` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Admin_Match_DeleteAll`()
BEGIN
	SET SQL_SAFE_UPDATES=0;
    
    TRUNCATE TABLE `Timeline`;
	TRUNCATE TABLE `LiveMatch`;
	TRUNCATE TABLE `Match`;
    
	SET SQL_SAFE_UPDATES=1;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Admin_Match_DeleteById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Admin_Match_DeleteById`(IN id varchar(45))
BEGIN
	SET SQL_SAFE_UPDATES=0;
    DELETE FROM `Timeline` as TL WHERE TL.MatchId = id;
	DELETE FROM `LiveMatch` as LM WHERE LM.Id = id;
	DELETE FROM `Match` as MC WHERE MC.Id = id;
	SET SQL_SAFE_UPDATES=1;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Admin_Match_DeleteLiveMatch` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Admin_Match_DeleteLiveMatch`(IN limitCount INT)
BEGIN
	SET SQL_SAFE_UPDATES=0;
    DROP TEMPORARY TABLE IF EXISTS LiveMatchIds;
    CREATE TEMPORARY TABLE LiveMatchIds SELECT Id
		FROM `LiveMatch`
        LIMIT limitCount;
        
    DELETE FROM `Timeline` WHERE `MatchId` IN (SELECT Id from LiveMatchIds);
	DELETE FROM `LiveMatch` where Id IN (SELECT Id from LiveMatchIds);
	DELETE FROM `Match` where Id IN (SELECT Id from LiveMatchIds);
    
	DROP TEMPORARY TABLE IF EXISTS LiveMatchIds;
	SET SQL_SAFE_UPDATES=1;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Admin_Match_DeleteMatchByDate` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Admin_Match_DeleteMatchByDate`(IN fromDate DATETIME, IN toDate DATETIME, IN limitCount INT)
BEGIN
	SET SQL_SAFE_UPDATES=0;
    DROP TEMPORARY TABLE IF EXISTS matchIds;
    CREATE TEMPORARY TABLE matchIds SELECT Id
		FROM 
			(SELECT Id, EventDate FROM `Match` as NonLiveMatch  WHERE Id NOT IN (SELECT  Id FROM    LiveMatch as LM)
			UNION ALL (SELECT Id, EventDate FROM `LiveMatch` as LM)) as AllMatches
		WHERE EventDate >= fromDate and EventDate <= toDate
        LIMIT limitCount;
        
    DELETE FROM `Timeline` WHERE `MatchId` IN (SELECT Id from matchIds);
	DELETE FROM `Match` where Id IN (SELECT Id from matchIds);
	DELETE FROM `LiveMatch` where Id IN (SELECT Id from matchIds);
    
	DROP TEMPORARY TABLE IF EXISTS matchIds;
	SET SQL_SAFE_UPDATES=1;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Auto_JenkinsSp` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Auto_JenkinsSp`()
BEGIN
	SELECT Id FROM score247_local_test.Match LIMIT 10;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `CI_CleanUpData` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `CI_CleanUpData`()
BEGIN
	DELETE FROM score247_local_test.Match;
    DELETE FROM score247_local_test.LiveMatch;
    DELETE FROM score247_local_test.Timeline;
    DELETE FROM score247_local_test.Odds;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Commentary_Archive` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `Commentary_Archive`()
BEGIN
	INSERT INTO `Commentary_Archived`
    SELECT * FROM `Commentary` as T
    WHERE T.CreatedTime <= (UTC_TIMESTAMP() - INTERVAL 5 DAY)
    ON DUPLICATE KEY UPDATE
		`Value` = T.`Value`;
    
    DELETE FROM `Commentary`
    WHERE CreatedTime <= (UTC_TIMESTAMP() - INTERVAL 5 DAY);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `League_GetActive` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `League_GetActive`()
BEGIN
	SELECT `Id`, `Name`, `Order`, `CategoryId`, `Country` as 'CountryName', `CountryCode`, `IsInternational`, `Region` FROM `League` WHERE `IsActive` = '1';
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `League_GetById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `League_GetById`(IN leagueId varchar(255))
BEGIN
	SELECT * FROM `League` WHERE `Id` = leagueId;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `League_InsertCountryLeagues` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `League_InsertCountryLeagues`(IN sportId INT, IN leagues MEDIUMTEXT, IN language TEXT)
BEGIN
	DECLARE i INT DEFAULT 0;                                                                                                                                                    
    DECLARE e INT DEFAULT JSON_LENGTH(leagues);

    WHILE i < e DO
		INSERT INTO `League`(`Id`,`Name`, `Region`,`CategoryId`, `Country`, `CountryCode`,`IsInternational`, `Language` )
			VALUES (
				JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Id'))),
				JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Name'))),
				JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CategoryId'))),
				JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Region'))),
				JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CountryName'))),
				JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CountryCode'))),
				'0',
				language)
			ON DUPLICATE KEY UPDATE
				`Name` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Name'))),
				`Region` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Region'))),
				`CategoryId` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CategoryId'))),
				`CountryCode` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CountryCode'))),
				`Country` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CountryName')));
        
        -- Increment the loop variable                                                                                                                                                        
        SET i = i + 1;
    END WHILE;
    
    -- Update ContryCode for minor leagues
    SET SQL_SAFE_UPDATES = 0;
	Update `League` Set `CountryCode` = '' WHERE `IsMajor` = 0;
	SET SQL_SAFE_UPDATES = 1;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `League_InsertInternationalLeagues` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `League_InsertInternationalLeagues`(IN sportId INT, IN leagues MEDIUMTEXT, IN language TEXT)
BEGIN
	DECLARE i INT DEFAULT 0;                                                                                                                                                    
    DECLARE e INT DEFAULT JSON_LENGTH(leagues);
    
    WHILE i < e DO
		INSERT INTO `League`(`Id`,`Name`, `Region`,`CategoryId`,`IsInternational`, `Language` )
			VALUES (
				JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Id'))),
				JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Name'))),
				JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CategoryId'))),
				JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Region'))),
				'1',
				language)
			ON DUPLICATE KEY UPDATE
				`Name` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Name'))),
				`Region` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Region'))),
				`CategoryId` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CategoryId')));
	
        -- Increment the loop variable                                                                                                                                                        
        SET i = i + 1;
    END WHILE;
    
    -- Update ContryCode for minor leagues
    SET SQL_SAFE_UPDATES = 0;
	Update `League` Set `CountryCode` = '' WHERE `IsMajor` = 0;
	SET SQL_SAFE_UPDATES = 1;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `League_InsertLeagues` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `League_InsertLeagues`(IN sportId INT, IN leagues MEDIUMTEXT, IN language TEXT)
BEGIN
	DECLARE i INT DEFAULT 0;                                                                                                                                                    
    DECLARE e INT DEFAULT JSON_LENGTH(leagues);

    WHILE i < e DO
		IF (JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].IsInternational'))) = true)
        THEN
			INSERT INTO `League`(`Id`,`Name`, `Region`,`CategoryId`,`IsInternational`, `Language` )
			VALUES (
				JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Id'))),
				JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Name'))),
				JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CategoryId'))),
				JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Region'))),
				'1',
				language)
			ON DUPLICATE KEY UPDATE
				`Name` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Name'))),
				`Region` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Region'))),
				`CategoryId` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CategoryId')));
        ELSE
			INSERT INTO `League`(`Id`,`Name`, `Region`,`CategoryId`, `Country`, `CountryCode`,`IsInternational`, `Language` )
				VALUES (
					JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Id'))),
					JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Name'))),
					JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CategoryId'))),
					JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Region'))),
					JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CountryName'))),
					JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CountryCode'))),
					'0',
					language)
				ON DUPLICATE KEY UPDATE
					`Name` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Name'))),
					`Region` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].Region'))),
					`CategoryId` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CategoryId'))),
					`CountryCode` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CountryCode'))),
					`Country` = JSON_UNQUOTE(JSON_EXTRACT(leagues, CONCAT('$[', i, '].CountryName')));
        END IF;
        
        -- Increment the loop variable                                                                                                                                                        
        SET i = i + 1;
    END WHILE;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `LiveMatch_GetBySportId` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_GetBySportId`(IN sportId INT, IN language TEXT)
BEGIN
	SELECT JSON_REPLACE(LM.Value, '$.ModifiedTime', LM.ModifiedTime) as `Value`
    FROM LiveMatch as LM 
    INNER JOIN `League` as League ON LM.LeagueId = League.Id 	
	WHERE LM.SportId = sportId 
		AND LM.`Language` = language 
        AND League.IsActive = 1
    ORDER BY League.Order, LM.EventDate, LM.LeagueId, LM.Id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `LiveMatch_InsertOrRemove` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_InsertOrRemove`(
	IN sportId INT, 
    IN language TEXT, 
    IN newMatches MEDIUMTEXT, 
    IN removedMatchIds TEXT)
BEGIN
	DECLARE ni INT DEFAULT 0;     
    DECLARE ne INT DEFAULT JSON_LENGTH(newMatches);
    
    DECLARE di INT DEFAULT 0;     
    DECLARE de INT DEFAULT JSON_LENGTH(removedMatchIds);
    
    -- Insert new live matches
    WHILE ni < ne DO          
		SET @Id = JSON_UNQUOTE(JSON_EXTRACT(newMatches, CONCAT('$[', ni, '].MatchId')));
    
		IF NOT EXISTS (SELECT 1 FROM `LiveMatch` WHERE Id = @Id AND Language = language) 
        THEN			
			 INSERT INTO `LiveMatch`
			 SELECT 
				Id, 
				JSON_SET(`Value`,  '$.MatchResult', JSON_EXTRACT(newMatches, CONCAT('$[', ni, '].MatchResult'))) as `Value`,
				`Language`,
				`SportId`,
				LeagueId,
				EventDate,
				Region,
				now(),
				now()
				FROM `Match` as M
			 WHERE M.`SportId` = sportId 
				AND M.Id = @Id
                AND M.Language = language;
		 END IF;
         
        -- Increment the loop variable                                                                                                                                                        
        SET ni = ni + 1;
    END WHILE; 
    
    -- Delete live matches
    WHILE di < de DO 
		SET @DeleteId = JSON_UNQUOTE(JSON_EXTRACT(removedMatchIds, CONCAT('$[', di, '].MatchId')));         
		DELETE FROM `LiveMatch` WHERE Id = @DeleteId AND SportId = sportId AND Language = language;         
        -- Increment the loop variable                                                                                                                                                        
        SET di = di + 1;
    END WHILE;     
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `LiveMatch_RemoveById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_RemoveById`(IN matchId varchar(50))
BEGIN
	DELETE FROM `LiveMatch` WHERE Id = matchId;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `LiveMatch_UpdateCurrentPeriodStartTime` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_UpdateCurrentPeriodStartTime`(IN sportId INT, IN matchId VARCHAR(45), IN currentPeriodStartTime TINYTEXT)
BEGIN
	UPDATE `LiveMatch` as LM
	SET `Value` = JSON_SET(`Value`,  '$.CurrentPeriodStartTime', JSON_UNQUOTE(currentPeriodStartTime))
	WHERE LM.`SportId` = sportId AND LM.Id = matchId;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `LiveMatch_UpdateLastTimeline` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_UpdateLastTimeline`(
	IN sportId INT(11), 
    IN matchId VARCHAR(45), 
    IN timelineEvent TEXT)
BEGIN
	UPDATE `LiveMatch` as LM
	SET `Value` = JSON_SET(`Value`,  '$.LatestTimeline', JSON_EXTRACT(timelineEvent, '$'))
	WHERE LM.`SportId` = sportId AND LM.Id = matchId;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `LiveMatch_UpdateMatchResult` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_UpdateMatchResult`(
	IN sportId INT(11), 
    IN matchId VARCHAR(45), 
    IN matchResult TEXT)
BEGIN
	IF EXISTS (SELECT * FROM `Match` AS M WHERE M.Id = matchId FOR SHARE) 
    THEN
		 IF EXISTS (SELECT * FROM `LiveMatch` AS LM WHERE LM.Id = matchId FOR SHARE) 
         THEN
			UPDATE `LiveMatch` as LM
			SET `Value` = JSON_SET(`Value`,  '$.MatchResult', JSON_EXTRACT(matchResult, '$'))
			WHERE LM.`SportId` = sportId AND LM.Id = matchId;
		 ELSE
			 INSERT INTO `LiveMatch`
			 SELECT 
				Id, 
				JSON_SET(`Value`,  '$.MatchResult', JSON_EXTRACT(matchResult, '$')) as `Value`,
				`Language`,
				`SportId`,
				LeagueId,
				EventDate,
				Region,
				now(),
				now()
				FROM `Match` as M
			 WHERE M.`SportId` = sportId AND M.Id = matchId;
		 END IF;
    END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `LiveMatch_UpdateTeamRedCards` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_UpdateTeamRedCards`(
			IN sportId INT, 
            IN matchId VARCHAR(45), 
            IN teamIndex INT, 
            IN redCards INT,
            IN yellowRedCards INT)
BEGIN
	UPDATE `LiveMatch` as LM
    SET Value = JSON_SET(Value, 
			CONCAT('$.Teams[', teamIndex ,'].Statistic.RedCards'), redCards,
            CONCAT('$.Teams[', teamIndex ,'].Statistic.YellowRedCards'), yellowRedCards)
    WHERE LM.`SportId` = sportId AND LM.`Id` = matchId;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `LiveMatch_UpdateTeamStatistic` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `LiveMatch_UpdateTeamStatistic`(IN sportId INT, IN matchId VARCHAR(45), IN teamIndex INT, IN statistic TEXT)
BEGIN
	UPDATE `LiveMatch` as LM
    SET Value = JSON_SET(Value, CONCAT('$.Teams[', teamIndex ,'].Statistic'), JSON_EXTRACT(statistic, '$'))
    WHERE LM.`SportId` = sportId AND LM.`Id` = matchId;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Match_Archive` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Match_Archive`()
BEGIN
	INSERT INTO Match_Archived
		SELECT * FROM `Match` as M
        WHERE M.EventDate <= (UTC_TIMESTAMP() - INTERVAL 5 DAY)
		ON DUPLICATE KEY UPDATE
			Value = M.`Value`,
			EventDate = M.EventDate;
			
		DELETE FROM `Match`
		WHERE EventDate <= (UTC_TIMESTAMP() - INTERVAL 5 DAY);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Match_GetByDateRange` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Match_GetByDateRange`(IN sportId INT, IN fromDate DATETIME, IN toDate DATETIME, IN language TEXT)
BEGIN
	CREATE TEMPORARY TABLE IF NOT EXISTS temp_match AS
    (SELECT NonLiveMatch.`Value`, NonLiveMatch.EventDate, NonLiveMatch.LeagueId, NonLiveMatch.Id, NonLiveMatch.ModifiedTime 
		FROM `Match` as NonLiveMatch 
		INNER JOIN `League` as League ON NonLiveMatch.LeagueId = League.Id
		WHERE NonLiveMatch.Id NOT IN
			(
			SELECT  Id
			FROM    LiveMatch as LM
			)
		   AND NonLiveMatch.SportId = sportId
			AND NonLiveMatch.EventDate >=  fromDate
			AND NonLiveMatch.EventDate <=  toDate
			AND League.IsActive =  '1'
			AND NonLiveMatch.`Language` = language)
		UNION ALL 
		(SELECT Value, EventDate, LeagueId, Id, ModifiedTime FROM `LiveMatch` as LM
		 WHERE LM.SportId = sportId
			AND  LM.EventDate >=  fromDate
			AND LM.EventDate <=  toDate
			AND LM.`Language` = language);
    
    SELECT JSON_REPLACE(`Match`.Value,  '$.ModifiedTime', `Match`.ModifiedTime) as `Value`, `Match`.EventDate AS EventDate, `Match`.LeagueId AS LeagueId, `Match`.Id AS Id
    FROM temp_match AS `Match`
	INNER JOIN `League` as League ON `Match`.LeagueId = League.Id
	ORDER BY League.Order, `Match`.EventDate, `Match`.LeagueId, `Match`.Id;

	DROP TEMPORARY TABLE temp_match;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Match_GetById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Match_GetById`(IN Id VARCHAR(45), IN language TEXT)
BEGIN
	IF EXISTS (SELECT 1 FROM `LiveMatch` as LM WHERE LM.Id = Id AND LM.`Language` = language) 
	THEN
		SELECT `Value` FROM `LiveMatch` as LM  WHERE LM.Id = Id AND LM.`Language` = language;
	ELSE
		SELECT `Value` FROM `Match` as M  WHERE M.Id = Id AND M.`Language` = language;
	END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Match_GetCommentaries` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Match_GetCommentaries`(IN matchId VARCHAR(45), IN language VARCHAR(10))
BEGIN
	SELECT JSON_SET(T.`Value`,  '$.Commentaries', C.`Value`) AS Commentary
    FROM Timeline AS T
    LEFT JOIN Commentary AS C ON 		
		T.Id = C.TimelineId 
        AND T.MatchId = C.MatchId
    WHERE T.MatchId = matchId
		AND T.Language = language;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Match_GetLineups` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `Match_GetLineups`(IN matchId TEXT, IN language TEXT)
BEGIN
	SELECT `Value` FROM `Lineups` as LU  WHERE LU.MatchId = matchId AND LU.`Language` = language;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Match_GetMatches` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Match_GetMatches`()
BEGIN
	SELECT *
		FROM `Match`
        ORDER BY `EventDate` DESC;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Match_GetTimelineEvents` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Match_GetTimelineEvents`(IN matchId VARCHAR(45))
BEGIN
	SELECT `Value` 
    FROM Timeline AS T
    WHERE T.MatchId = matchId;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Match_InsertCommentary` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Match_InsertCommentary`(IN matchId TEXT, IN timelineId BIGINT, IN commentaries TEXT, IN language TEXT)
BEGIN
	IF NOT EXISTS (SELECT 1 
						FROM `Commentary` AS Commentary 
						WHERE  Commentary.MatchId = matchId 
							AND Commentary.TimelineId = timelineId 
                            AND Commentary.Language = language)
    THEN        
		 INSERT INTO `Commentary` (
			`MatchId`,
			`TimelineId`,
			`Value`,
			`Language`,
			`CreatedTime`,
			`ModifiedTime`
			)
			VALUES (			
				matchId,
				timelineId,
				JSON_UNQUOTE(JSON_EXTRACT(commentaries, '$')),
				language,
				now(),
				now());
	ELSE
			UPDATE `Commentary` AS Commentary
			SET 
				Commentary.`Value` = JSON_SET(`Value`,  '$', JSON_EXTRACT(commentaries, '$')),
                Commentary.`ModifiedTime` = now()
			WHERE Commentary.MatchId = matchId
				AND Commentary.TimelineId = timelineId 
				AND Commentary.Language = language;		
    END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Match_InsertMatch` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Match_InsertMatch`(
	IN id varchar(45),
    IN matchValue MEDIUMTEXT,
    IN sportId varchar(45),
    IN matchLanguage varchar(10),
    IN leagueId varchar(45),
    IN eventDate varchar(100),
    IN region varchar(10))
BEGIN
	INSERT INTO `Match`
		(`Id`, `Value`, `SportId`, `Language`, `LeagueId`, `EventDate`, `Region`, `CreatedTime`, `ModifiedTime`)
        VALUE (id, matchValue, sportId, matchLanguage, leagueId, STR_TO_DATE(eventDate,'%Y-%m-%dT%H:%i:%s+00:00'), region, now(), now());
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Match_InsertMatches` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Match_InsertMatches`(IN sportId INT, IN matches MEDIUMTEXT, IN language TEXT)
BEGIN
	DECLARE i INT DEFAULT 0;                                                                                                                                                    
    DECLARE e INT DEFAULT JSON_LENGTH(matches);

    WHILE i < e DO                                                                                                              
        INSERT INTO `Match` VALUES (
			JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].Id'))),
			JSON_EXTRACT(matches, CONCAT('$[', i, ']')),
            language, 
            sportId, 
            JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].League.Id'))), 
            STR_TO_DATE(JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].EventDate'))),'%Y-%m-%dT%H:%i:%s+00:00'),
            JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].Region'))),
            now(),
            now())
		ON DUPLICATE KEY UPDATE
            Value = JSON_EXTRACT(matches, CONCAT('$[', i, ']')),
            EventDate = STR_TO_DATE(JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].EventDate'))),'%Y-%m-%dT%H:%i:%s+00:00'),
            ModifiedTime = now();
        -- Increment the loop variable                                                                                                                                                        
        SET i = i + 1;
    END WHILE;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Match_InsertOrUpdateLineups` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `Match_InsertOrUpdateLineups`(IN matchId TEXT, IN lineups TEXT, IN language TEXT)
BEGIN
	 INSERT INTO `Lineups` VALUES (
			matchId,
			JSON_UNQUOTE(JSON_EXTRACT(lineups, '$')),
            language,
            now(),
            now())
	 ON DUPLICATE KEY UPDATE
			`Value` = JSON_UNQUOTE(JSON_EXTRACT(lineups, '$')),
			ModifiedTime = now();
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Match_InsertOrUpdatePostMatches` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `Match_InsertOrUpdatePostMatches`(IN sportId INT, IN matches MEDIUMTEXT, IN language TEXT)
BEGIN
	DECLARE i INT DEFAULT 0;                                                                                                                                                    
    DECLARE e INT DEFAULT JSON_LENGTH(matches);
	DECLARE Id VARCHAR(45);

    WHILE i < e DO        
        
			SET @Id = JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].Id')));
            
			IF(NOT EXISTS (SELECT 1 FROM  `Match` AS M WHERE M.Id = @Id AND M.Language = language))
			THEN
				INSERT INTO `Match` VALUES (
					@Id,
					JSON_EXTRACT(matches, CONCAT('$[', i, ']')),
					language, 
					sportId, 
					JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].League.Id'))), 
					STR_TO_DATE(JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].EventDate'))),'%Y-%m-%dT%H:%i:%s+00:00'),
					JSON_UNQUOTE(JSON_EXTRACT(matches, CONCAT('$[', i, '].Region'))),
					now(),
					now());
			ELSE
				UPDATE `Match` as M
				SET 
					Value = JSON_REPLACE(Value,  '$.MatchResult', JSON_EXTRACT(matches, CONCAT('$[', i, '].MatchResult'))),
					ModifiedTime = now()
				WHERE M.SportId = sportId
					AND M.Id = @Id
                    AND M.Language = language;
			END IF;
            
        -- Increment the loop variable                                                                                                                                                        
        SET i = i + 1;        
      
    END WHILE;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Match_InsertTimeline` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Match_InsertTimeline`(IN matchId TEXT, IN timeline TEXT, IN language TEXT)
BEGIN
	 INSERT INTO `Timeline` VALUES (
			JSON_UNQUOTE(JSON_EXTRACT(timeline, '$.Id')),
			matchId,
			JSON_UNQUOTE(JSON_EXTRACT(timeline, '$')),
            language,
            now(),
            now())
	 ON DUPLICATE KEY UPDATE
			`Value` = JSON_UNQUOTE(JSON_EXTRACT(timeline, '$')),
			ModifiedTime = now();
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Match_UpdateCoverage` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Match_UpdateCoverage`(
	IN sportId INT, 
    IN matchId VARCHAR(45),     
    IN coverage TEXT)
BEGIN
	UPDATE `Match` as M
    SET 
		Value = JSON_SET(Value,  '$.Coverage', JSON_EXTRACT(coverage, '$')),
        ModifiedTime = now()
	WHERE M.SportId = sportId
		AND M.Id = matchId;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Match_UpdateMatchResult` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Match_UpdateMatchResult`(
	IN sportId INT, 
    IN matchId VARCHAR(45), 
    IN matchResult TEXT)
BEGIN
	UPDATE `Match` as M
    SET 
		Value = JSON_REPLACE(Value,  '$.MatchResult', JSON_EXTRACT(matchResult, '$')),
        ModifiedTime = now()
	WHERE M.SportId = sportId
		AND M.Id = matchId;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Match_UpdateMatchResultAndMigrateLiveData` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Match_UpdateMatchResultAndMigrateLiveData`(
	IN sportId INT, 
    IN matchId VARCHAR(45), 
    IN matchResult TEXT)
BEGIN	
   IF EXISTS (SELECT * FROM `LiveMatch` WHERE SportId = sportId AND Id = matchId FOR SHARE) 
   THEN
		UPDATE `LiveMatch` as LM
		SET  LM.`Value` = JSON_REPLACE(Value,  '$.MatchResult', JSON_EXTRACT(matchResult, '$'))
		WHERE LM.SportId = sportId AND LM.Id = matchId;
        
		UPDATE `Match` as M
		SET  M.`Value` = 
				(SELECT `Value` FROM `LiveMatch` as LM 
				WHERE LM.SportId = sportId
				AND LM.Id = matchId)
		WHERE M.SportId = sportId
			AND M.Id = matchId;
	END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Match_UpdateRefereeAndAttendance` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Match_UpdateRefereeAndAttendance`(
	IN sportId INT, 
    IN matchId VARCHAR(45), 
    IN referee VARCHAR(200),
    IN attendance INT,
    IN language VARCHAR(10))
BEGIN
	UPDATE `Match` as M
    SET 
		Value = JSON_REPLACE(Value,  '$.Referee', referee, '$.Attendance', attendance),
        ModifiedTime = now()
	WHERE M.SportId = sportId
		AND M.Id = matchId
        AND M.Language = language;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Odds_Archive` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Odds_Archive`()
BEGIN
	INSERT INTO Odds_Archived
						(`CreatedTime`,
						`Value`,
						`MatchId`,
						`BetTypeId`,
						`BookmakerId`)
    SELECT o.`CreatedTime`, o.`Value`, o.`matchId`, o.`BetTypeId`, o.`BookmakerId` FROM `Odds` AS o
    WHERE o.CreatedTime <= (UTC_TIMESTAMP() - INTERVAL 10 DAY);
        
	DELETE FROM `Odds` AS o
    WHERE o.CreatedTime <= (UTC_TIMESTAMP() - INTERVAL 10 DAY);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Odds_GetOdds` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Odds_GetOdds`(
	IN matchId VARCHAR(45), 
    IN betTypeId INT,
    IN bookmakerId VARCHAR(20))
BEGIN
	SELECT `Value` 
    FROM Odds AS o
    WHERE o.MatchId = matchId
		AND (betTypeId = 0 OR o.BetTypeId = betTypeId)
        AND (bookmakerId = '' OR o.BookmakerId = bookmakerId);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Odds_InsertOdds` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Odds_InsertOdds`(
		IN matchId VARCHAR(45), 
        IN oddsList TEXT)
BEGIN
		DECLARE oddsIndex INT DEFAULT 0;
		DECLARE totalOdds INT DEFAULT JSON_LENGTH(oddsList);
		
		WHILE oddsIndex < totalOdds DO       
        
        INSERT INTO `Odds`
						(`CreatedTime`,
						`Value`,
						`matchId`,
						`BetTypeId`,
						`BookmakerId`)
						VALUES
						(UTC_TIMESTAMP(6),
						JSON_EXTRACT(oddsList, CONCAT('$[', oddsIndex, ']')),
						matchId,
						JSON_EXTRACT(oddsList, CONCAT('$[', oddsIndex, '].Id')),
						JSON_UNQUOTE(JSON_EXTRACT(oddsList, CONCAT('$[', oddsIndex, '].Bookmaker.Id'))));
			-- Increment the loop variable                                                                                                                                                        
			SET oddsIndex = oddsIndex + 1;
		END WHILE;
	END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Team_GetHeadToHeads` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Team_GetHeadToHeads`(IN homeTeamId TINYTEXT, IN awayTeamId TINYTEXT, IN `language` TINYTEXT)
BEGIN
	SELECT `Value` FROM HeadToHead as H2H
    WHERE H2H.HomeTeamId = homeTeamId AND H2H.AwayTeamId = awayTeamId AND H2H.Language = `language`;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Team_InsertOrUpdateHeadToHead` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Team_InsertOrUpdateHeadToHead`(IN homeTeamId TINYTEXT, IN awayTeamId TINYTEXT, IN `match` MEDIUMTEXT, IN matchId TINYTEXT, IN `language` TINYTEXT)
BEGIN
	INSERT INTO `HeadToHead`
    VALUES(
		homeTeamId, 
        awayTeamId, 
        matchId, 
        `language`,
        `match`,
        now(),
		now())
    ON DUPLICATE KEY
    UPDATE `Value` = `match`;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Timeline_Archive` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`user`@`%` PROCEDURE `Timeline_Archive`()
BEGIN
	INSERT INTO `Timeline_Archived`
    SELECT * FROM `Timeline` as T
    WHERE T.CreatedTime <= (UTC_TIMESTAMP() - INTERVAL 5 DAY)
    ON DUPLICATE KEY UPDATE
		`Value` = T.`Value`;
    
    DELETE FROM `Timeline`
    WHERE CreatedTime <= (UTC_TIMESTAMP() - INTERVAL 5 DAY);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2019-11-05 16:35:51

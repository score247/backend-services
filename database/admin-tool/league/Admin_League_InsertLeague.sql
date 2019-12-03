DROP procedure IF EXISTS `Admin_League_InsertLeague`;

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
END
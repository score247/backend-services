CREATE DEFINER=`root`@`%` PROCEDURE `Admin_News_GetAll`(IN languageCode varchar(10), IN sportId int(11))
BEGIN
	SELECT
		NS.Title,
        NS.Content,
        NS.ImageSource,
        NS.Link,
        NS.Author,
        NS.PublishedDate,
        NS.Provider,
        IF(NS.IsShown = '1', true, false) as 'IsShown'
    FROM `News` as NS
    WHERE NS.Language = languageCode
		AND NS.SportId = sportId;
END
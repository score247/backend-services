CREATE DEFINER=`root`@`%` PROCEDURE `Admin_News_UpdateNews`(
	IN title varchar(500), 
	IN content mediumtext,
    IN imageSource varchar(250),
    IN imageName varchar(250),
    IN author varchar(250),
    IN publishedDate datetime,
    IN provider varchar(250),
    IN link varchar(250),
    IN isShown bool,
    IN languageCode varchar(10))
BEGIN
	UPDATE `News` as NS
    SET NS.Content = content,
		NS.Title = title,
        NS.ImageSource = imageSource,
        NS.ImageName = imageName,
        NS.Author = author,
        NS.Provider = provider,
        NS.IsShown = isShown,
        NS.ModifiedTime = now()
    WHERE NS.Link = link
		AND NS.Language = languageCode;
END
DROP procedure IF EXISTS `UserFavorite_UpdatePushEventStatus`;

CREATE DEFINER=`root`@`%` PROCEDURE `UserFavorite_UpdatePushEventStatus`(IN userId varchar(255), IN isEnable bool)
BEGIN
	UPDATE `UserFavorite` as UF
    SET
		UF.EnablePush = isEnable
	WHERE UF.UserId = userId;
END
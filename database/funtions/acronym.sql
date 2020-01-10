CREATE DEFINER=`dev`@`%` FUNCTION `acronym`(str text) RETURNS text CHARSET utf8
begin
    declare result text default '';
    set result = initials( UPPER(str), '[[:alnum:]]' );
    return result;
end
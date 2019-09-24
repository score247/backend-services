SELECT * FROM `Match` as M where M.Id = '00752b03-c883-11e9-b99c-0242ac110013'  LIMIT 0, 100000  ;
SELECT * FROM score247_dev_machine.`LiveMatch` as M  LIMIT 0, 100000  ;
SELECT * FROM `Match` as M  LIMIT 0, 100000  ;
SELECT * FROM `Match` as M  WHERE EventDate = '2019-08-26 00:00:00';
SELECT * FROM score247_dev_machine.`Timeline` WHERE MatchId = 'sr:match:373656828';
DELETE FROM `Match` WHERE EventDate >= '2019-08-27 00:00:00' AND EventDate <= '2019-08-29 00:00:00' LIMIT 50;

TRUNCATE `Match`;
INSERT `Match` 
SELECT * FROM score247_local_main.`Match` LIMIT 50;

CALL CI_InsertMatchesForPerformanceTesting('2019-08-17 00:00:00', 15);
CALL CI_InsertMatchesForPerformanceTesting('2019-08-18 00:00:00', 30);
CALL CI_InsertMatchesForPerformanceTesting('2019-08-19 00:00:00', 200);
CALL CI_InsertMatchesForPerformanceTesting('2019-08-20 00:00:00', 250);
CALL CI_InsertMatchesForPerformanceTesting('2019-08-21 00:00:00', 240);
CALL CI_InsertMatchesForPerformanceTesting('2019-08-28 00:00:00', 50);
CALL CI_InsertMatchesForPerformanceTesting('2019-08-26 00:00:00', 300);

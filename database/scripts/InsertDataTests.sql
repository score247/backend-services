SELECT * FROM `Match` WHERE EventDate = '2019-08-23 00:00:00';
TRUNCATE `Match`;
INSERT `Match` 
SELECT * FROM score247_local_main.`Match` LIMIT 50;

CALL CI_InsertMatchesForPerformanceTesting('2019-08-17 00:00:00', 15);
CALL CI_InsertMatchesForPerformanceTesting('2019-08-18 00:00:00', 30);
CALL CI_InsertMatchesForPerformanceTesting('2019-08-19 00:00:00', 60);
CALL CI_InsertMatchesForPerformanceTesting('2019-08-20 00:00:00', 120);
CALL CI_InsertMatchesForPerformanceTesting('2019-08-21 00:00:00', 240);
CALL CI_InsertMatchesForPerformanceTesting('2019-08-22 00:00:00', 480);
CALL CI_InsertMatchesForPerformanceTesting('2019-08-23 00:00:00', 960);

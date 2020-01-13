UPDATE `Match` as M
SET M.`Value` = JSON_INSERT(M.`Value`, '$.LeagueGroupStage', null);

UPDATE `Match` as M
SET M.`Value` = JSON_INSERT(M.`Value`, '$.GroupName', null);


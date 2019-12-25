INSERT INTO `score247_local_test`.`LeagueGroupStage`
(`GroupStageName`,
`LeagueId`,
`LeagueSeasonId`,
`Value`,
`Language`,
`CreatedTime`,
`ModifiedTime`)
SELECT
	distinct JSON_UNQUOTE(JSON_EXTRACT(M.Value, "$.LeagueGroupName")) as GroupStage, 
	M.LeagueId,
    M.LeagueSeasonId,
    CONCAT('{',
		   CONCAT('"GroupStageName":',M.Value -> '$.LeagueGroupName',''),
		   CONCAT(',"LeagueRound":',M.Value -> "$.LeagueRound",''),
		   CONCAT(',"LeagueSeasonId":"',M.LeagueSeasonId,'"'),
		   CONCAT(',"LeagueId":"',M.LeagueId,'"')
		 ,'}') as GroupStageValue,
	M.Language,
    now(),
    now()
FROM score247_local_test.Match AS M
WHERE M.Value -> "$.LeagueRound.Type.DisplayName" <> 'group'
OR JSON_UNQUOTE(JSON_EXTRACT(M.Value, "$.LeagueRound.Group")) <> 'null'
ON DUPLICATE KEY UPDATE
	`Value` = CONCAT('{',
		   CONCAT('"GroupStageName":',M.Value -> '$.LeagueGroupName',''),
		   CONCAT(',"LeagueRound":',M.Value -> "$.LeagueRound",''),
		   CONCAT(',"LeagueSeasonId":"',M.LeagueSeasonId,'"'),
		   CONCAT(',"LeagueId":"',M.LeagueId,'"')
		 ,'}'),
	`ModifiedTime` = now();

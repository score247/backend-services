INSERT INTO `score247_local_test`.`LeagueGroupStage`
(`GroupStageName`,
`LeagueId`,
`Language`,
`CreatedTime`,
`ModifiedTime`)
SELECT
	distinct M.Value -> "$.LeagueGroupName" as GroupStage, 
	M.LeagueId, 	
	M.Language, 
    now(),
    now()
FROM score247_local_test.Match AS M
WHERE M.Value -> "$.LeagueRound.Type.DisplayName" <> 'group'
OR JSON_UNQUOTE(JSON_EXTRACT(M.Value, "$.LeagueRound.Group")) <> 'null';

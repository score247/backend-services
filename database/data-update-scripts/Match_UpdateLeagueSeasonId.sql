UPDATE `Match`
	SET LeagueSeasonId = JSON_UNQUOTE(`Value` -> "$.LeagueSeason.Id"),
    ModifiedTime = NOW();
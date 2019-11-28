UPDATE `Timeline`
	SET Type = JSON_UNQUOTE(`Value` -> "$.Type.DisplayName"),
    ModifiedTime = NOW();
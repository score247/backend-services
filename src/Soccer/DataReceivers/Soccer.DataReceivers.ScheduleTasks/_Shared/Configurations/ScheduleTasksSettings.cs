namespace Soccer.DataReceivers.ScheduleTasks.Shared.Configurations
{
    public class ScheduleTasksSettings
    {
#pragma warning disable S107 // Methods should not have too many parameters

        public ScheduleTasksSettings(
            int queueBatchSize = 0,
            int fetchMatchResultDateSpan = 0,
            int fetchMatchScheduleDateSpan = 0,
            string fetchOddsScheduleJobCron = "0 0/6 * * *",
            string fetchOddsChangeJobCron = "*/5 * * * *",
            int fetchOddsChangeMinuteInterval = 5,
            string fetchLiveMatchCron = "*/1 * * * *",
            string fetchLiveMatchesTimelineCron = "*/5 * * * *",
            string fetchPostMatchesCron = " 0 0/6 * * *",
            string fetchPreMatchesCron = " 0 0/6 * * *",
            string fetchLeaguesCron = "0 0 12 1 * ?",
            string fetchMatchLineupsCron = "0 * * * *",
            string cleanMajorLeaguesCacheCron = "0 0 1 1 *",
            int fetchMatchesByDateDelayedHours = 2)
        {
            QueueBatchSize = queueBatchSize;
            FetchMatchResultDateSpan = fetchMatchResultDateSpan;
            FetchMatchScheduleDateSpan = fetchMatchScheduleDateSpan;
            FetchOddsScheduleJobCron = fetchOddsScheduleJobCron;
            FetchOddsChangeJobCron = fetchOddsChangeJobCron;
            FetchOddsChangeMinuteInterval = fetchOddsChangeMinuteInterval;
            FetchLiveMatchesCron = fetchLiveMatchCron;
            FetchLiveMatchesTimelineCron = fetchLiveMatchesTimelineCron;
            FetchPostMatchesCron = fetchPostMatchesCron;
            FetchPreMatchesCron = fetchPreMatchesCron;
            FetchLeaguesCron = fetchLeaguesCron;
            FetchMatchLineupsCron = fetchMatchLineupsCron;
            CleanMajorLeaguesCacheCron = cleanMajorLeaguesCacheCron;
            FetchMatchesByDateDelayedHours = fetchMatchesByDateDelayedHours;
        }

#pragma warning restore S107 // Methods should not have too many parameters

        public int QueueBatchSize { get; set; }

        public int FetchMatchResultDateSpan { get; set; }

        public int FetchMatchScheduleDateSpan { get; set; }

        public string FetchOddsScheduleJobCron { get; set; }

        public string FetchOddsChangeJobCron { get; set; }

        public int FetchOddsChangeMinuteInterval { get; set; }

        public string FetchLiveMatchesCron { get; set; }

        public string FetchLiveMatchesTimelineCron { get; set; }

        public string FetchPostMatchesCron { get; set; }

        public string FetchPreMatchesCron { get; set; }

        public string FetchLeaguesCron { get; set; }

        public string FetchMatchLineupsCron { get; set; }

        public string CleanMajorLeaguesCacheCron { get; set; }

        public int FetchMatchesByDateDelayedHours { get; set; }
    }
}
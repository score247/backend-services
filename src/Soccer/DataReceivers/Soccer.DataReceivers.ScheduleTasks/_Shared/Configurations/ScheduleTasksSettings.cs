namespace Soccer.DataReceivers.ScheduleTasks.Shared.Configurations
{
    public class ScheduleTasksSettings
    {
        public ScheduleTasksSettings(
            int queueBatchSize = 0,
            int fetchMatchResultDateSpan = 0,
            int fetchMatchScheduleDateSpan = 0,
            string fetchOddsScheduleJobCron = "0 0/6 * * *",
            string fetchOddsChangeJobCron = "*/5 * * * *",
            int fetchOddsChangeMinuteInterval = 5)
        {
            QueueBatchSize = queueBatchSize;
            FetchMatchResultDateSpan = fetchMatchResultDateSpan;
            FetchMatchScheduleDateSpan = fetchMatchScheduleDateSpan;
            FetchOddsScheduleJobCron = fetchOddsScheduleJobCron;
            FetchOddsChangeJobCron = fetchOddsChangeJobCron;
            FetchOddsChangeMinuteInterval = fetchOddsChangeMinuteInterval;
        }

        public int QueueBatchSize { get; set; }

        public int FetchMatchResultDateSpan { get; set; }

        public int FetchMatchScheduleDateSpan { get; set; }

        public string FetchOddsScheduleJobCron { get; set; }

        public string FetchOddsChangeJobCron { get; set; }

        public int FetchOddsChangeMinuteInterval { get; set; }
    }
}
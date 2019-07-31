namespace Soccer.DataReceivers.ScheduleTasks.Shared.Configurations
{
    public class ScheduleTasksSettings
    {
        public ScheduleTasksSettings(
            int queueBatchSize = 0,
            int fetchMatchResultDateSpan = 0,
            int fetchMatchScheduleDateSpan = 0)
        {
            QueueBatchSize = queueBatchSize;
            FetchMatchResultDateSpan = fetchMatchResultDateSpan;
            FetchMatchScheduleDateSpan = fetchMatchScheduleDateSpan;
        }

        public int QueueBatchSize { get; set; }

        public int FetchMatchResultDateSpan { get; set; }

        public int FetchMatchScheduleDateSpan { get; set; }
    }
}
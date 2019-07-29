namespace Soccer.DataReceivers.ScheduleTasks._Shared.Configurations
{
    public class ScheduleTasksSettings
    {
        public int QueueBatchSize { get; set; }

        public int FetchMatchResultDateSpan { get; set; }

        public int FetchMatchScheduleDateSpan { get; set; }
    }
}
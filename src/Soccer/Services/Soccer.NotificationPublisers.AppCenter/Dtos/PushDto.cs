namespace Soccer.NotificationServices.AppCenter.Dtos
{ 
    public class PushDto
    {
        public TargetDto notification_target { get; set; }

        public ContentDto notification_content { get; set; }
    }
}

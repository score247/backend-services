namespace Soccer.Database.AccountSettings.Commands
{
    public class UpdateNotificationStatusCommand : BaseCommand
    {
        public UpdateNotificationStatusCommand(string userId, bool isEnable)
        {
            UserId = userId;
            IsEnable = isEnable;
        }

        public string UserId { get; }

        public bool IsEnable { get; }

        public override string GetSettingKey()
            => "UserFavorite_UpdatePushEventStatus";

        public override bool IsValid()
            => !string.IsNullOrEmpty(UserId);
    }
}
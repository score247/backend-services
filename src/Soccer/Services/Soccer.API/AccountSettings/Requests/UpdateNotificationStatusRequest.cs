using MediatR;

namespace Soccer.API.AccountSettings.Requests
{
    public class UpdateNotificationStatusRequest : IRequest<bool>
    {
        public UpdateNotificationStatusRequest(string userId, bool isEnable)
        {
            UserId = userId;
            IsEnable = isEnable;
        }

        public string UserId { get; }

        public bool IsEnable { get; }
    }
}
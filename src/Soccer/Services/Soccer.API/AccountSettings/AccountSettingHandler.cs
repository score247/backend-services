using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Soccer.API.AccountSettings.Requests;

namespace Soccer.API.AccountSettings
{
    public class AccountSettingHandler :
        IRequestHandler<UpdateNotificationStatusRequest, bool>
    {
        private readonly IAccountSettingCommandService accountSettingCommandService;

        public AccountSettingHandler(IAccountSettingCommandService accountSettingCommandService)
        {
            this.accountSettingCommandService = accountSettingCommandService;
        }

        public async Task<bool> Handle(UpdateNotificationStatusRequest request, CancellationToken cancellationToken)
        => (await accountSettingCommandService.UpdateNotificationStatus(request.UserId, request.IsEnable)) > 0;
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Soccer.API.AccountSettings;
using Soccer.API.AccountSettings.Requests;
using Xunit;

namespace Soccer.API.Tests.AccountSettings
{
    public class AccountSettingHandlerTests
    {
        private readonly AccountSettingHandler accountSettingHandler;
        private readonly IAccountSettingCommandService accountSettingCommandService;

        public AccountSettingHandlerTests()
        {
            accountSettingCommandService = Substitute.For<IAccountSettingCommandService>();

            accountSettingHandler = new AccountSettingHandler(accountSettingCommandService);
        }

        [Fact]
        public async Task HandleUpdateNotificationStatusRequest_Execute_GetLive()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var isEnable = true;
            var request = new UpdateNotificationStatusRequest(userId, isEnable);

            // Act
            await accountSettingHandler.Handle(request, new CancellationToken());

            // Assert
            await accountSettingCommandService.Received(1).UpdateNotificationStatus(userId, isEnable);
        }
    }
}
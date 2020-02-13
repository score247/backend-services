using System;
using System.Threading.Tasks;
using MediatR;
using NSubstitute;
using Soccer.API.AccountSettings;
using Soccer.API.AccountSettings.Requests;
using Soccer.Core.Shared.Enumerations;
using Xunit;

namespace Soccer.API.Tests.AccountSettings
{
    public class AccountSettingControllerTests
    {
        private readonly IMediator mediator;
        private readonly AccountSettingController accountSettingController;

        public AccountSettingControllerTests()
        {
            mediator = Substitute.For<IMediator>();
            accountSettingController = new AccountSettingController(mediator);
        }

        [Fact]
        public async Task UpdateNotificationStatus_Excused_MediatorSend()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var isEnable = false;
            var language = Language.English;

            // Act
            await accountSettingController.UpdateNotificationStatus(userId, isEnable, language);

            // Assert
            await mediator
                .Received(1)
                .Send(Arg.Is<UpdateNotificationStatusRequest>(request => request.UserId == userId && request.IsEnable == isEnable));
        }
    }
}
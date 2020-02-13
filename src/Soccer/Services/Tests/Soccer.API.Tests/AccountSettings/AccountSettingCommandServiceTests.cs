using System;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using NSubstitute;
using Soccer.API.AccountSettings;
using Soccer.Database.AccountSettings.Commands;
using Xunit;

namespace Soccer.API.Tests.AccountSettings
{
    public class AccountSettingCommandServiceTests
    {
        private readonly AccountSettingCommandService accountSettingCommandService;
        private readonly IDynamicRepository dynamicRepository;

        public AccountSettingCommandServiceTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            accountSettingCommandService = new AccountSettingCommandService(dynamicRepository);
        }

        [Fact]
        public async Task UpdateNotificationStatus__ExecuteAsyncFromRepository()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var isEnable = true;

            // Act
            await accountSettingCommandService.UpdateNotificationStatus(userId, isEnable);

            // Assert
            await dynamicRepository
                .Received(1)
                .ExecuteAsync(Arg.Is<UpdateNotificationStatusCommand>(command => command.IsEnable == isEnable && command.UserId == userId));
        }
    }
}
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Soccer.Database.AccountSettings.Commands;

namespace Soccer.API.AccountSettings
{
    public interface IAccountSettingCommandService
    {
        Task<int> UpdateNotificationStatus(string userId, bool isEnable);
    }

    public class AccountSettingCommandService : IAccountSettingCommandService
    {
        private readonly IDynamicRepository dynamicRepository;

        public AccountSettingCommandService(IDynamicRepository dynamicRepository)
        => this.dynamicRepository = dynamicRepository;

        public Task<int> UpdateNotificationStatus(string userId, bool isEnable)
            => dynamicRepository.ExecuteAsync(new UpdateNotificationStatusCommand(userId, isEnable));
    }
}
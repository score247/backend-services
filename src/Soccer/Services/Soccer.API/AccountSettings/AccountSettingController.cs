using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Soccer.API.AccountSettings.Requests;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.AccountSettings
{
    [Route("api/soccer/{language}/settings/{userId}")]
    [ApiController]
    public class AccountSettingController : ControllerBase
    {
        private readonly IMediator mediator;

        public AccountSettingController(IMediator mediator)
            => this.mediator = mediator;

        [HttpPost]
        [Route("notification")]
        public async Task<bool> UpdateNotificationStatus(string userId, [FromQuery]bool isEnable, string language = Language.English)
            => await mediator.Send(new UpdateNotificationStatusRequest(userId, isEnable));
    }
}
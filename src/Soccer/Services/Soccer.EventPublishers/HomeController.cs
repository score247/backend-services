using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Soccer.EventPublishers.Hubs;

namespace Soccer.EventPublishers
{
    public class HomeController : ControllerBase
    {
        private readonly IHubContext<SoccerHub> hubContext;

        public HomeController(IHubContext<SoccerHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public IActionResult Index()
        {
            var clients = JsonConvert.SerializeObject(SoccerHub.GetAllActiveConnections(), Formatting.Indented);

            return new ContentResult
            {
                Content = $"Clients Info: {clients}",
                ContentType = "text/html"
            };
        }

        public async Task<IActionResult> MockPushEvents()
        {
            var clients = JsonConvert.SerializeObject(SoccerHub.GetAllActiveConnections(), Formatting.Indented);

            return new ContentResult
            {
                Content = $"Sent to All Client: {clients}"
            };
        }
    }
}
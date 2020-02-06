using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Soccer.NotificationPublishers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return new ContentResult
            {
                Content = "This is Notification Publishers",
                ContentType = "text/html"
            };
        }
    }
}

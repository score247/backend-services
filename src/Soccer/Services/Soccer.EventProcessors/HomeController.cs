using Microsoft.AspNetCore.Mvc;

namespace Soccer.EventProcessors
{
    public class HomeController : ControllerBase
    {
        public IActionResult Index()
        {
            return new JsonResult("Event Processors Started!");
        }
    }
}
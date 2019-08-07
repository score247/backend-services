namespace Soccer.EventProcessors
{
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : ControllerBase
    {
        public IActionResult Index()
        {
            return new JsonResult("Event Publishers started!");
        }
    }
}
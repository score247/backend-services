namespace Soccer.EventProcessors
{
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : ControllerBase
    {
#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        public IActionResult Index()
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
            => new JsonResult("Event Processors Started!");
    }
}
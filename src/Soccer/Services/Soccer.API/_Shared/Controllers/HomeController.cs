using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Soccer.API._Shared.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return new ContentResult
            {
                Content = "Home Page"
            };
        }
    }
}
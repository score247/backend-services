using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Soccer.EventProcessors
{
    public class HomeController : ControllerBase
    {
        public IActionResult Index()
        {
            return new JsonResult("Started!");
        }
    }
}
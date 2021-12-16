using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DriverService.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        [Obsolete]
        public readonly IHostingEnvironment _hostingEnv;

        [Obsolete]
        public HomeController(IHostingEnvironment hostingEnv)
        {
            _hostingEnv = hostingEnv;
        }
        [HttpGet]
        [Obsolete]
        public string Get()
        {
            return "Driver service started successfully. Environment - " + _hostingEnv.EnvironmentName +"";
        }
    }
}

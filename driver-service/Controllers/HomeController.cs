using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace driver_service.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        public readonly IWebHostEnvironment HostingEnv;

        public HomeController(IWebHostEnvironment hostingEnv)
        {
            HostingEnv = hostingEnv;
        }
        [HttpGet]
        public string Get()
        {
            return "Driver service started successfully. Environment - " + HostingEnv.EnvironmentName + "";
        }
    }
}

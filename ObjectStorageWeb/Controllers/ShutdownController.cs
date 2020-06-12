using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace ObjectStorageWeb.Controllers
{
    public class ShutdownController : Controller
    {
        private IHostApplicationLifetime _applicationLifetime;

        public ShutdownController(IHostApplicationLifetime applicationLifetime)
        {
            _applicationLifetime = applicationLifetime;
        }

        public IActionResult Index()
        {
            _applicationLifetime.StopApplication();
            return new EmptyResult();
        }
    }
}
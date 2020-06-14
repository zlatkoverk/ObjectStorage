using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace ObjectStorageWeb.Controllers
{
    [Authorize]
    public class ShutdownController : Controller
    {
        private IHostApplicationLifetime _applicationLifetime;

        public ShutdownController(IHostApplicationLifetime applicationLifetime)
        {
            _applicationLifetime = applicationLifetime;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Confirm()
        {
            _applicationLifetime.StopApplication();
            return new EmptyResult();
        }
    }
}
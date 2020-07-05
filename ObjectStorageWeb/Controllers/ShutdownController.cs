using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using ObjectStorage;

namespace ObjectStorageWeb.Controllers
{
    [Authorize]
    public class ShutdownController : Controller
    {
        private IHostApplicationLifetime _applicationLifetime;
        private Storage _storage;

        public ShutdownController(IHostApplicationLifetime applicationLifetime, Storage storage)
        {
            _applicationLifetime = applicationLifetime;
            _storage = storage;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Confirm()
        {
            _storage.dump();
            _applicationLifetime.StopApplication();
            return new ContentResult()
            {
                Content = @"
<html>
<head>
<meta http-equiv=""refresh"" content=""7;url=/"" />
</head>
<body>
Please wait a moment
</body>
</html>
",
                ContentType = "text/html"
            };
        }
    }
}
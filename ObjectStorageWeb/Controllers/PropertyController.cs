using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ObjectStorage;
using ObjectStorage.MetaModel;
using ObjectStorageWeb.Models;

namespace ObjectStorageWeb.Controllers
{
    public class PropertyController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Storage _storage;

        public PropertyController(ILogger<HomeController> logger, Storage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        [HttpGet("/property/{className}")]
        public IActionResult Index(string className)
        {
            return View(new Property());
        }

        [HttpGet("/property/{id}/delete")]
        public IActionResult Delete(string id)
        {
            var c = _storage.deleteProperty(Guid.Parse(id));
            return RedirectPermanent($"/class/{c.Name}");
        }

        [HttpPost("/property/{className}"), ValidateAntiForgeryToken]
        public IActionResult Save(string className, [FromForm] Property p)
        {
            _storage.addProperty(className, p);
            return RedirectPermanent($"/class/{className}");
        }
    }
}
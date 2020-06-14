using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ObjectStorage;
using ObjectStorage.MetaModel;
using ObjectStorageWeb.Models;

namespace ObjectStorageWeb.Controllers
{
    [Authorize]
    public class PropertyController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Storage _storage;
        private readonly StorageState _state;

        public PropertyController(ILogger<HomeController> logger, Storage storage, StorageState state)
        {
            _logger = logger;
            _storage = storage;
            _state = state;
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
            _state.Valid = false;
            return RedirectPermanent($"/class/{c.Name}");
        }

        [HttpPost("/property/{className}"), ValidateAntiForgeryToken]
        public IActionResult Save(string className, [FromForm] Property p)
        {
            _storage.addProperty(className, p);
            _state.Valid = false;
            return RedirectPermanent($"/class/{className}");
        }
    }
}
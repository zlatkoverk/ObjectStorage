using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ObjectStorage;
using ObjectStorage.MetaModel;
using ObjectStorageWeb.Models;

namespace ObjectStorageWeb.Controllers
{
    public class ClassController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Storage _storage;

        public ClassController(ILogger<HomeController> logger, Storage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        [HttpGet("/class/{className}")]
        [HttpGet("/class/")]
        public IActionResult Edit(string className = "")
        {
            var c = _storage.getClasses().Find(c => c.Name.ToLower().Equals(className.ToLower()));
            if (c == null)
            {
                c = new Class() {Name = className};
            }

            return View(c);
        }

        
        [HttpPost("/class/")]
        public IActionResult Save([FromForm] Class c)
        {
            _storage.addDefinition(c);
            return RedirectPermanent($"/class/{c.Name}");
        }
    }
}
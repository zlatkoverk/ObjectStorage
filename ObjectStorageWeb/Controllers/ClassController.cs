using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ObjectStorage;
using ObjectStorage.MetaModel;
using ObjectStorageWeb.Models;

namespace ObjectStorageWeb.Controllers
{
    [Authorize]
    public class ClassController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Storage _storage;
        private readonly StorageState _state;

        public ClassController(ILogger<HomeController> logger, Storage storage, StorageState state)
        {
            _logger = logger;
            _storage = storage;
            _state = state;
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
            _state.Valid = false;
            return RedirectPermanent($"/class/{c.Name}");
        }
        
        [HttpGet("/class/{className}/delete/{id}")]
        public IActionResult Delete(string className, string id, [FromQuery(Name = "redirectTo")] string redirectTo = "")
        {
            _storage.removeElement(className, id);
            return RedirectPermanent(redirectTo == "" ? $"/class/{className}/overview" : redirectTo);
        }

        [HttpPost("/class/{className}/presentationProperty")]
        public IActionResult PresentationProperty(string className, [FromForm] Dictionary<string, string> data)
        {
            var c = _storage.getClasses().Find(c => c.Name.ToLower().Equals(className.ToLower()));

            Guid id;
            if (Guid.TryParse(data["PresentationProperty"], out id))
            {
                var p = c.Properties.First(p => p.Id == id);
                if (p != null)
                {
                    c.PresentationProperty = p;
                    _storage.addDefinition(c);
                    _state.Valid = false;
                }
            }

            return RedirectPermanent($"/class/{c.Name}");
        }

        [AllowAnonymous]
        [HttpGet("/class/{className}/overview")]
        public IActionResult Overview(string className)
        {
            var v = new OverviewViewModel();
            v.Class = _storage.getClasses().Find(c => c.Name.ToLower().Equals(className.ToLower()));
            v.Elements = _storage.getEntities(v.Class).Select(e =>
                    v.Class.Properties.ToDictionary(p => p.Name, v => e.GetType().GetProperty(v.Name).GetValue(e)))
                .ToList();
            v.Options = new Dictionary<string, List<object>>();
            foreach (var property in v.Class.Properties)
            {
                var c = _storage.getClasses().Find(c => c.Name.ToLower().Equals(property.Type.ToLower()));
                if (c != null)
                {
                    v.Options[property.Name] = _storage.getEntities(c).ToList();
                }
            }

            return View(v);
        }

        [HttpPost("/class/{className}")]
        public IActionResult Add(string className, [FromForm] Dictionary<string, string> data,
            [FromQuery(Name = "redirectTo")] string redirectTo = "")
        {
            if (!_state.Valid)
            {
                return RedirectPermanent("/shutdown");
            }

            foreach (KeyValuePair<string, string> kvp in data)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }

            _storage.addElement(className, data);
            return RedirectPermanent(redirectTo == "" ? $"/class/{className}/overview" : redirectTo);
        }
    }
}
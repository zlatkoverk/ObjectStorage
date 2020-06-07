using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet("/class/{className}/overview")]
        public IActionResult Overview(string className)
        {
            var v = new OverviewViewModel();
            v.Class = _storage.getClasses().Find(c => c.Name.ToLower().Equals(className.ToLower()));
            v.Elements = _storage.getEntities(v.Class).Select(e => v.Class.Properties.ToDictionary(p=>p.Name, v=>e.GetType().GetProperty(v.Name).GetValue(e))).ToList();
            return View(v);
        }
        
        [HttpPost("/class/{className}")]
        public IActionResult Add(string className, [FromForm] Dictionary<string, string> data)
        {
            foreach (KeyValuePair<string, string> kvp in data)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
            _storage.addElement(className, data);
            return RedirectPermanent($"/class/{className}/overview");
        }
    }
}
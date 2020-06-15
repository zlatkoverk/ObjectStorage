using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DotLiquid;
using Microsoft.AspNetCore.Mvc;
using ObjectStorage;
using ObjectStorageWeb.Models;

namespace ObjectStorageWeb.Controllers
{
    public class EntityController : Controller
    {
        private readonly Storage _storage;

        public EntityController(Storage storage, StorageState state)
        {
            _storage = storage;
        }

        public IActionResult Index()
        {
            var className = "Processor";
            Template.RegisterSafeType(typeof(OverviewViewModel), new[] {"Class", "Elements", "Options"});

            var template = Template.Parse(System.IO.File.ReadAllText(Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "Template/Overview.liquid")));

            var v = new OverviewViewModel();
            v.Class = _storage.getClasses().Find(c => c.Name.ToLower().Equals(className.ToLower()));
            v.Elements = _storage.getEntities(v.Class).Select(e =>
                {
                    var dict = v.Class.Properties.ToDictionary(p => p.Name,
                        v => e.GetType().GetProperty(v.Name).GetValue(e));
                    dict.Add("Id", e.GetType().GetProperty("Id").GetValue(e));
                    return dict;
                })
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

            var render = template.Render(Hash.FromAnonymousObject(new {Model = v}));
            return new ContentResult()
            {
                Content = render, ContentType = "text/html"
            };
        }
    }
}
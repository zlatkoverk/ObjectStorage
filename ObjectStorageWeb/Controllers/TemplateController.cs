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

        [HttpGet("/entity/{className}/overview")]
        public IActionResult Index(string className)
        {
            var o = _storage.getClasses().Find(c => c.Name.ToLower().Equals(className.ToLower()));
            Template.RegisterSafeType(typeof(OverviewViewModel), new[] {"Class", "Elements", "Options"});
            Template.RegisterSafeType(typeof(OptionViewModel), new[] {"Id", "Value"});

            var v = OverviewViewModel.create(_storage, className);
            var template = Template.Parse(o.OverviewTemplate);

            var render = template.Render(Hash.FromAnonymousObject(new {Model = v}));
            return new ContentResult()
            {
                Content = render, ContentType = "text/html"
            };
        }

        [HttpGet("/entity/{className}/{entityId}")]
        public IActionResult Index(string className, string entityId)
        {
            var o = _storage.getClasses().Find(c => c.Name.ToLower().Equals(className.ToLower()));
            Template.RegisterSafeType(typeof(DetailsViewModel), new[] {"Class", "Element", "Options"});
            Template.RegisterSafeType(typeof(OptionViewModel), new[] {"Id", "Value"});

            var v = DetailsViewModel.create(_storage, className, entityId);
            var render = Template.Parse(o.DetailsTemplate).Render(Hash.FromAnonymousObject(new {Model = v}));

            return new ContentResult()
            {
                Content = render, ContentType = "text/html"
            };
        }
    }
}
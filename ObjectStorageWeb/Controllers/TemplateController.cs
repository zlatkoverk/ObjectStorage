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

            var template = Template.Parse(o.OverviewTemplate);

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
            v.Options = new Dictionary<string, List<OptionViewModel>>();
            foreach (var property in v.Class.Properties)
            {
                var c = _storage.getClasses().Find(c => c.Name.ToLower().Equals(property.Type.ToLower()));
                if (c != null)
                {
                    v.Options[property.Name] =
                        _storage.getEntities(c).Select(e => new OptionViewModel() {Object = e}).ToList();                }
            }

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

            Guid id;
            if (!Guid.TryParse(entityId, out id))
            {
                return new NotFoundResult();
            }

            var template = Template.Parse(o.DetailsTemplate);

            var v = new DetailsViewModel();
            v.Class = _storage.getClasses().Find(c => c.Name.ToLower().Equals(className.ToLower()));
            v.Element = _storage.getEntities(v.Class)
                .FindAll(e => e.GetType().GetProperty("Id").GetValue(e).Equals(id)).Select(e =>
                {
                    var dict = v.Class.Properties.ToDictionary(p => p.Name,
                        v => e.GetType().GetProperty(v.Name).GetValue(e));
                    dict.Add("Id", e.GetType().GetProperty("Id").GetValue(e));
                    return dict;
                }).First();
            v.Options = new Dictionary<string, List<OptionViewModel>>();
            foreach (var property in v.Class.Properties)
            {
                var c = _storage.getClasses().Find(c => c.Name.ToLower().Equals(property.Type.ToLower()));
                if (c != null)
                {
                    v.Options[property.Name] =
                        _storage.getEntities(c).Select(e => new OptionViewModel() {Object = e}).ToList();
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using DotLiquid;
using Microsoft.AspNetCore.Mvc;
using ObjectStorage;
using ObjectStorageWeb.Models;

namespace ObjectStorageWeb.Controllers
{
    public class ApiController : Controller
    {
        private readonly Storage _storage;

        public ApiController(Storage storage, StorageState state)
        {
            _storage = storage;
        }

        [HttpGet("/api/{className}")]
        public IActionResult Meta(string className)
        {
            return new ContentResult()
            {
                Content = JsonSerializer.Serialize(OverviewViewModel.create(_storage, className).Class),
                ContentType = "application/json"
            };
        }

        [HttpGet("/api/{className}/all")]
        public IActionResult All(string className)
        {
            return new ContentResult()
            {
                Content = JsonSerializer.Serialize(OverviewViewModel.create(_storage, className, true).Elements),
                ContentType = "application/json"
            };
        }

        [HttpGet("/api/{className}/options")]
        public IActionResult Options(string className)
        {
            return new ContentResult()
            {
                Content = JsonSerializer.Serialize(OverviewViewModel.create(_storage, className, true).Options),
                ContentType = "application/json"
            };
        }

        [HttpGet("/api/{className}/{entityId}")]
        public IActionResult Details(string className, string entityId)
        {
            var dvm = DetailsViewModel.create(_storage, className, entityId, true);
            if (dvm == null || dvm.Element == null)
            {
                return new NotFoundResult();
            }

            return new ContentResult()
            {
                Content = JsonSerializer.Serialize(dvm.Element), ContentType = "application/json"
            };
        }

        [HttpPost("/api/{className}")]
        public IActionResult Add(string className, [FromBody] Dictionary<string, string> data)
        {
            if (!_storage.addElement(className, data))
            {
                return new BadRequestResult();
            }
            return new NoContentResult();
        }

        [HttpDelete("/api/{className}/{id}")]
        public IActionResult Delete(string className, string id)
        {
            _storage.removeElement(className, id);
            return new NoContentResult();
        }
    }
}
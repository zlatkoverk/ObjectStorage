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

        [HttpGet("/api/{className}/overview")]
        public IActionResult Index(string className)
        {
            return new ContentResult()
            {
                Content = JsonSerializer.Serialize(OverviewViewModel.create(_storage, className)),
                ContentType = "application/json"
            };
        }

        [HttpGet("/api/{className}/{entityId}")]
        public IActionResult Index(string className, string entityId)
        {
            var dvm = DetailsViewModel.create(_storage, className, entityId);
            if (dvm == null)
            {
                return new NotFoundResult();
            }

            return new ContentResult()
            {
                Content = JsonSerializer.Serialize(dvm), ContentType = "application/json"
            };
        }
    }
}
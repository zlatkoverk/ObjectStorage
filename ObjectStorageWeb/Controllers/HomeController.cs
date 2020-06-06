using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ObjectStorage;
using ObjectStorage.DbContext;
using ObjectStorageWeb.Models;

namespace ObjectStorageWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Storage _storage;
        private readonly ModelDbContext _modelContext;

        public HomeController(ILogger<HomeController> logger, ModelDbContext modelContext)
        {
            _logger = logger;
            _modelContext = modelContext;
        }

        public IActionResult Index()
        {
            return View(new IndexViewModel() {Classes = _modelContext.Classes.ToList()});
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
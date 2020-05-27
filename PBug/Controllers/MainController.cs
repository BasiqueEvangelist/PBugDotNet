using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PBug.Data;
using PBug.Models;

namespace PBug.Controllers
{
    public class MainController : Controller
    {
        private readonly ILogger<MainController> _logger;
        private readonly PBugContext Db;

        public MainController(ILogger<MainController> logger, PBugContext db)
        {
            _logger = logger;
            Db = db;
        }



        [Route("/error/")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
        [Route("/403")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error403()
        {
            return View();
        }
        [Route("/404")]
        public IActionResult Error404()
        {
            return View();
        }
        [Route("/400")]
        public IActionResult Error400()
        {
            return View();
        }
    }
}

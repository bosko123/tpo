using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using web.Models;
using web.Services;

namespace web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public JsonFileProductService ProductService;
        public IEnumerable<Product> ProductsList { get; set; }

        public HomeController(ILogger<HomeController> logger, JsonFileProductService productService)
        {
            _logger = logger;
            ProductService = productService;
        }

        public void OnGet()
        {
            ProductsList = ProductService.GetProducts();
            Console.WriteLine(ProductsList);
            ViewBag.P = ProductsList;
        }

        public IActionResult Index()
        {
            ProductsList = ProductService.GetProducts();
            ViewBag.productsList = ProductsList;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Products()
        {
            ProductsList = ProductService.GetProducts();
            ViewBag.productsList = ProductsList;
            return View();
        }

        public IActionResult DetailsSamsung()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

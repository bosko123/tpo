using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using web.Models;
using web.Services;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Net.Http;
using System.Security.Cryptography;

namespace web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public JsonFileProductService ProductService;
        public IEnumerable<Product> ProductsList { get; set; }
        public string result;

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

            if (TempData["data"] != null) {

                User user = JsonSerializer.Deserialize<User>(TempData["data"].ToString());
                TempData["user"] = user.ToString();

            }

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

        public IActionResult Register()
        {
            if (TempData["data"] == null) {
                return View();
            }

            if (TempData["errors"] != null) {
                int i = 0;
                foreach (string error in JsonSerializer.Deserialize<List<string>>(TempData["errors"].ToString()))
                {
                    ModelState.AddModelError("error" + i, error);
                    i++;
                }
            }
            RegisterModel model = JsonSerializer.Deserialize<RegisterModel>(TempData["data"].ToString());
            return View("Register", model);
        }

        public IActionResult Login()
        {
            if (TempData["data"] == null) {
                return View();
            }

            if (TempData["errors"] != null) {
                int i = 0;
                foreach (string error in JsonSerializer.Deserialize<List<string>>(TempData["errors"].ToString()))
                {
                    ModelState.AddModelError("error" + i, error);
                    i++;
                }
            }
            LoginModel model = JsonSerializer.Deserialize<LoginModel>(TempData["data"].ToString());
            return View("Login", model);
        }

        public async Task<string> sandJsonRequest(string Json, string type) {

            using (var client = new HttpClient())
            {
                // This would be the like http://www.uber.com
                client.BaseAddress = new Uri("http://tpo.ski-javornik.si");

                // serialize your json using newtonsoft json serializer then add it to the StringContent
                var content = new StringContent(Json, Encoding.UTF8, "application/json");

                // method address would be like api/callUber:SomePort for example
                var result = await client.PostAsync(type, content);
                string resultContent = await result.Content.ReadAsStringAsync();
                return resultContent;
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<ActionResult> registerSubmit(RegisterModel model) {

            if (ModelState.IsValid) {

                if (model.password != model.confirmPassword) {

                    List<string> list = new List<string>();
                    list.Add("Password and Confirm password must be the same");

                    TempData["data"] = model.ToString();
                    TempData["errors"] = JsonSerializer.Serialize(list);

                    return RedirectToAction("Register", model);

                }

                var pass = model.password;
                model.password = SHA512(model.password);

                Console.WriteLine(model.password);

                string result = await sandJsonRequest(model.ToString(), "sign_up");

                Dictionary<string, string> values = JsonSerializer.Deserialize<Dictionary<string, string>>(result);

                if (values.ContainsKey("error")) {
                    
                    List<string> list = new List<string>();
                    list.Add(values["error"]);

                    model.password = pass;
                    TempData["data"] = model.ToString();

                    TempData["errors"] = JsonSerializer.Serialize(list);

                    return RedirectToAction("Register", model);

                }

            }
            else {

                string str = model.ToString();
                var errorList = (from item in ModelState
                                where item.Value.Errors.Any()
                                select item.Value.Errors[0].ErrorMessage).ToList();
                TempData["data"] = str;
                TempData["errors"] = JsonSerializer.Serialize(errorList);

                return RedirectToAction("Register", model);

            }

            return View("Index", model);

        }

        [HttpPost]
        public async Task<ActionResult> loginSubmit(LoginModel model) {

            if (ModelState.IsValid) {

                var pass = model.password;

                model.password = SHA512(model.password);
                string result = await sandJsonRequest(model.ToString(), "sign_in");

                Dictionary<string, string> values = JsonSerializer.Deserialize<Dictionary<string, string>>(result);

                if (values.ContainsKey("error")) {
                    
                    List<string> list = new List<string>();
                    list.Add(values["error"]);

                    model.password = pass;
                    TempData["data"] = model.ToString();

                    TempData["errors"] = JsonSerializer.Serialize(list);

                    return RedirectToAction("Login", model);

                }

                TempData["data"] = result;

            }
            else {

                string str = model.ToString();

                var errorList = (from item in ModelState
                                where item.Value.Errors.Any()
                                select item.Value.Errors[0].ErrorMessage).ToList();
                TempData["data"] = str;
                TempData["errors"] = errorList;

                return RedirectToAction("Login", model);

            }

            return RedirectToAction("Index");

        }

        public static string SHA512(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }
    }
}

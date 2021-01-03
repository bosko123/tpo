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
using Microsoft.AspNetCore.Http;

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
            //HttpContext.Session.SetString("name", null);
        }

        public void OnGet()
        {
            ViewBag.P = ProductsList;
        }

        public async Task<IActionResult> Index()
        {
            HttpContext.Session.SetInt32("userProducts", 1);
            ViewBag.productsList = ProductsList;

            if (TempData["data"] != null) {

                User user = JsonSerializer.Deserialize<User>(TempData["data"].ToString());
                TempData["user"] = user.ToString();
                HttpContext.Session.SetString("name", user.name);
                HttpContext.Session.SetString("surname", user.surname);
                HttpContext.Session.SetString("token", user.token);

            }

            var token = HttpContext.Session.GetString("token");
            if (token != null) {

                string result = await sandJsonRequestGetToken(token, "user_items");
                
                Dictionary<string, Object> values = JsonSerializer.Deserialize<Dictionary<string, Object>>(result);

                if (!values.ContainsKey("Result")/*!values["Result"].Equals("Token is invalid") && !values["Result"].Equals("No products")*/) {

                    Dictionary<string, IEnumerable<Product>> json = JsonSerializer.Deserialize<Dictionary<string, IEnumerable<Product>>>(result);
                    ProductsList = json["Products"];

                }

                return View(ProductsList);

            }
            
            return RedirectToAction("Products");
            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Products()
        {
            HttpContext.Session.SetInt32("userProducts", 0);
            string result = await sandJsonRequestGet("home_items");
            Dictionary<string, IEnumerable<Product>> json = JsonSerializer.Deserialize<Dictionary<string, IEnumerable<Product>>>(result);
            ProductsList = json["Products"];

            return View(ProductsList);
        }

        public async Task<IActionResult> Details(int? id)
        {
            Dictionary<string, int?> data = new Dictionary<string, int?>();
            data.Add("id", id);

            string token = HttpContext.Session.GetString("token");

            if (token != null) {

                string json = JsonSerializer.Serialize<Dictionary<string, int?>>(data);
                string result = await sandJsonRequestToken(json, "product", token);
                ProductDetails productDetails = JsonSerializer.Deserialize<ProductDetails>(result);
                
                if (productDetails.lower != null) {

                    productDetails.lowerInput = productDetails.lower.ToString();

                }

                if (productDetails.upper != null) {

                    productDetails.upperInput = productDetails.upper.ToString();

                }

                return View(productDetails);

            }

            return RedirectToAction("Login");

        }
        
        public async Task<IActionResult> RemoveProduct(int? id) {

            var token = HttpContext.Session.GetString("token");
            if (token != null) {

                string result = await sandJsonRequestGetToken(token, "user_items");
                
                Dictionary<string, Object> values = JsonSerializer.Deserialize<Dictionary<string, Object>>(result);
                Product Product = null;

                if (!values.ContainsKey("Result")/*!values["Result"].Equals("Token is invalid") && !values["Result"].Equals("No products")*/) {

                    Dictionary<string, IEnumerable<Product>> json = JsonSerializer.Deserialize<Dictionary<string, IEnumerable<Product>>>(result);
                    ProductsList = json["Products"];
                    
                    foreach (Product product in ProductsList) {
                        
                        if (product.id == id) {

                            Product = product;
                            break;

                        }

                    }

                }

                return View(Product);

            }

            return RedirectToAction("Login");

        }

        public async Task<IActionResult> AddProduct() {

            var url = HttpContext.Session.GetString("add");
            if (url != null) {

                AddProductModel apm = new AddProductModel();
                apm.url = url;

                return await addProductSubmit(apm);

            }

            var token = HttpContext.Session.GetString("token");

            if (token == null) {

                return RedirectToAction("login");

            }

            return View();

        }

        public IActionResult Logout() {

            HttpContext.Session.Remove("name");
            HttpContext.Session.Remove("surname");
            HttpContext.Session.Remove("token");
            return RedirectToAction("Index");

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

        public async Task<string> sandJsonRequestPost(string Json, string type) {

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

        public async Task<string> sandJsonRequestToken(string Json, string type, string token) {

            using (var client = new HttpClient())
            {
                // This would be the like http://www.uber.com
                client.BaseAddress = new Uri("http://tpo.ski-javornik.si");
                client.DefaultRequestHeaders.Add("x-access-token", token);

                // serialize your json using newtonsoft json serializer then add it to the StringContent
                var content = new StringContent(Json, Encoding.UTF8, "application/json");

                // method address would be like api/callUber:SomePort for example
                var result = await client.PostAsync(type, content);
                string resultContent = await result.Content.ReadAsStringAsync();
                return resultContent;
            }

        }

        public async Task<string> sandJsonRequestGetToken(string token, string type) {

            using (var client = new HttpClient())
            {
                // This would be the like http://www.uber.com
                var request = new HttpRequestMessage {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("http://tpo.ski-javornik.si/" + type)
                    //Content = new StringContent(Json, Encoding.UTF8, "application/json")
                };
                request.Headers.Add("x-access-token", token);

                // method address would be like api/callUber:SomePort for example
                var result = await client.SendAsync(request);
                //result.EnsureSuccessStatusCode();
                string resultContent = await result.Content.ReadAsStringAsync();
                return resultContent;
            }

        }

        public async Task<string> sandJsonRequestGet(string type) {

            using (var client = new HttpClient())
            {
                // This would be the like http://www.uber.com
                var request = new HttpRequestMessage {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("http://tpo.ski-javornik.si/" + type)
                    //Content = new StringContent(Json, Encoding.UTF8, "application/json")
                };

                // method address would be like api/callUber:SomePort for example
                var result = await client.SendAsync(request);
                //result.EnsureSuccessStatusCode();
                string resultContent = await result.Content.ReadAsStringAsync();
                return resultContent;
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<ActionResult> RemoveProductSubmit(int? id) {

            Dictionary<string, int?> data = new Dictionary<string, int?>();
            data.Add("id", id);

            string token = HttpContext.Session.GetString("token");

            if (token != null) {

                string json = JsonSerializer.Serialize<Dictionary<string, int?>>(data);
                string result = await sandJsonRequestToken(json, "remove_product", token);

                return RedirectToAction("Index");

            }

            return RedirectToAction("Login");

        }

        [HttpPost]
        public async Task<ActionResult> thresholdSubmit(ProductDetails model) {

            SetThreshold threshold = new SetThreshold();
            threshold.id = model.id;

            double lower = double.Parse(model.lowerInput, System.Globalization.CultureInfo.InvariantCulture);
            double upper = double.Parse(model.upperInput, System.Globalization.CultureInfo.InvariantCulture);

            if (model.lowerInput != null) {

                threshold.spodnja = lower;

            }

            if (model.upperInput != null) {

                threshold.zgornja = upper;

            }

            var token = HttpContext.Session.GetString("token");
            if (token == null) {

                return RedirectToAction("Login");

            }

            string json = threshold.ToString();
            string result = await sandJsonRequestToken(json, "conditions_product", token);

            Console.WriteLine(json);
            Console.WriteLine(result);

            return RedirectToAction("Details", new {id = model.id});

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

                string result = await sandJsonRequestPost(model.ToString(), "sign_up");

                Dictionary<string, string> values = JsonSerializer.Deserialize<Dictionary<string, string>>(result);

                if (values["Result"].Equals("Missing data") || values["Result"].Equals("User already exists")) {
                    
                    List<string> list = new List<string>();
                    list.Add(values["Result"]);

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

            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> addProductSubmit(AddProductModel model) {

            var token = HttpContext.Session.GetString("token");

            if (token == null) {

                return RedirectToAction("Login");

            }

            string data = model.ToString();
            string result = await sandJsonRequestToken(data, "add_item", token);

            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<ActionResult> loginSubmit(LoginModel model) {

            if (ModelState.IsValid) {

                var pass = model.password;

                model.password = SHA512(model.password);
                string result = await sandJsonRequestPost(model.ToString(), "sign_in");

                Dictionary<string, string> values = JsonSerializer.Deserialize<Dictionary<string, string>>(result);

                if (values["Result"].Equals("User does not exist") || values["Result"].Equals("Wrong password")) {
                    
                    List<string> list = new List<string>();
                    list.Add(values["Result"]);

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

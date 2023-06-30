using Microsoft.AspNetCore.Mvc;
using NorthwesternDemo.Models;
using System.Diagnostics;
using Newtonsoft.Json;


namespace NorthwesternDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            return View();
        }
        public async Task<IActionResult> Api(string apiUrl) {
            
            if (string.IsNullOrEmpty(apiUrl)) {
                apiUrl = "https://www.balldontlie.io/api/v1/teams";
            }

            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode) {
                string jsonString = await response.Content.ReadAsStringAsync();
                //dynamic jsonData = JsonConvert.DeserializeObject(jsonString);
                //ViewBag.Result = jsonData;
                ViewBag.Url = apiUrl;
                ViewBag.Result = jsonString;
                return View();
            }
            return View();

        }

        public IActionResult MySql(string url)
        {

            return View();
        }

        public IActionResult Privacy()
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
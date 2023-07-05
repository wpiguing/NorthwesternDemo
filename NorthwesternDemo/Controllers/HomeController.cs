using Microsoft.AspNetCore.Mvc;
using NorthwesternDemo.Models;
using System.Diagnostics;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Configuration;

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
                // Sample Endpoints
                // apiUrl = "https://api.publicapis.org/entries?category=sports";
                apiUrl = "https://www.balldontlie.io/api/v1/teams";
            }

            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(apiUrl);

            ViewBag.StatusCode = response.StatusCode.ToString();
            ViewBag.Url = apiUrl;

            if (response.IsSuccessStatusCode) {
                string jsonString = await response.Content.ReadAsStringAsync();
                dynamic jsonData = JsonConvert.DeserializeObject(jsonString);
                ViewBag.Results = jsonData;
                return View();
            } else {
                //Handle other responses if needed
            }
            return View();

        }

        public IActionResult MySql(string sqlCon)
        {
            if (string.IsNullOrEmpty(sqlCon))
            {
                // localhost demo DB
                sqlCon = "server=localhost;user=root;password=Pongolilypesto!;database=demo";
            }

            ViewBag.Sql = sqlCon;

            try {
                using MySqlConnection connection = new MySqlConnection(sqlCon);
                connection.Open();

                // demo tables: stocks, cars
                string table = "cars";
                string query = $"SELECT * FROM {table} LIMIT 30";

                List<string> columnNames = new List<string>();
                List<List<object>> rows = new List<List<object>>();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        List<object> row = new List<object>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string column = reader.GetName(i);
                            if (!columnNames.Contains(column))
                            {
                                columnNames.Add(column);
                            }
                            object value = reader.GetValue(i);
                            row.Add(value);
                        }
                        rows.Add(row);
                    }
                    ViewBag.Rows = rows;
                    ViewBag.ColumnNames = columnNames;
                }
                connection.Close();

                return View();
            }
            catch (Exception ex) {
                ViewBag.Error = ex;
                return View();
            }
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
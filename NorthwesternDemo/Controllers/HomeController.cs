using Microsoft.AspNetCore.Mvc;
using NorthwesternDemo.Models;
using System.Diagnostics;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

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
                apiUrl = "https://swapi.dev/api/people";
            }

            ViewBag.Url = apiUrl;

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                ViewBag.StatusCode = response.StatusCode.ToString();

                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    dynamic jsonData = JsonConvert.DeserializeObject(jsonString);
                    ViewBag.Results = jsonData;
                }
                else
                {
                    //Handle other responses 
                }
            }
            return View();

        }

        public async Task<IActionResult> MySql(string sqlCon)
        {
            if (string.IsNullOrEmpty(sqlCon))
            {
                // localhost demo DB
                sqlCon = "server=localhost;user=root;password=Pongolilypesto!;database=demo";
            }

            ViewBag.Sql = sqlCon;

            try {
                // demo tables:  cars, stocks, emptyStockTable
                string table = "customers";
                string query = $"SELECT * FROM {table} LIMIT 30";

                MySqlViewModel model = new MySqlViewModel();

                using (MySqlConnection connection = new MySqlConnection(sqlCon)) {
                    
                    await connection.OpenAsync();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                        
                        if (!reader.HasRows){
                            // Handle no data
                        }

                        while (await reader.ReadAsync()) {

                            List<object> rowData = new List<object>();

                            for (int i = 0; i < reader.FieldCount; i++) {

                                string column = reader.GetName(i);

                                if (!model.ColumnNames.Contains(column)) {
                                    model.ColumnNames.Add(column);
                                }

                                object value = reader.GetValue(i);
                                rowData.Add(value);
                            }
                            model.Rows.Add(rowData);
                        }
                    }
                    connection.Close();
                }
                ViewBag.Table = table;
                return View(model);
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
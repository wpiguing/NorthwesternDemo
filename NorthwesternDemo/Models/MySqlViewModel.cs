namespace NorthwesternDemo.Models
{
    public class MySqlViewModel
    {
   
        public MySqlViewModel() {

            ColumnNames = new List<string>();
            Rows = new List<List<object>>();
            
        }
        public List<string> ColumnNames { get; set; }
        public List<List<object>> Rows { get; set; }
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace web.Models {

    public class Product {

        public int id { get; set; }
        public string image { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public bool condition { get; set; }
        public object lower { get; set; }
        public object upper { get; set; }

        public Product() {

        }

        public override string ToString() {

            return JsonSerializer.Serialize<Product>(this);

        }

    }

}
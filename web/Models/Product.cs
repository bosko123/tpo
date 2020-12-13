using System.Text.Json;
using System.Text.Json.Serialization;

namespace web.Models {

    public class Product {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public string ImageUrl { get; set; }

        public Product() {

        }

        public override string ToString() {

            return JsonSerializer.Serialize<Product>(this);

        }

    }

}
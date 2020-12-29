using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace web.Models {

    public class ProductDetails {

        public int id { get; set; }
        public string slika { get; set; }
        public string ime { get; set; }
        public double cena { get; set; }
        public Dictionary<string, double> stare_cene { get; set; }
        public string url { get; set; }

        public ProductDetails() {

        }

        public override string ToString() {

            return JsonSerializer.Serialize<ProductDetails>(this);

        }

    }

}
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace web.Models {

    public class ProductDetails {

        public int id { get; set; }
        public string image { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public Dictionary<string, Dictionary<string, Object>> prices_data { get; set; }

        public ProductDetails() {

        }

        public override string ToString() {

            return JsonSerializer.Serialize<ProductDetails>(this);

        }

    }

}
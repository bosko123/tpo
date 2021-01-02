using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace web.Models {

    public class ProductDetails {

        [Display(Name = "ID")]
        public int id { get; set; }
        public string image { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public Dictionary<string, Dictionary<string, Object>> prices_data { get; set; }
        public bool condition { get; set; }
        public object lower { get; set; }
        public object upper { get; set; }
        [JsonIgnore]
        [Display(Name = "Lower")]
        public string lowerInput { get; set; }
        [JsonIgnore]
        [Display(Name = "Upper")]
        public string upperInput { get; set; }

        public ProductDetails() {

        }

        public override string ToString() {

            return JsonSerializer.Serialize<ProductDetails>(this);

        }

    }

}
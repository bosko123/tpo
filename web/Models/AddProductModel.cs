using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace web.Models {

    public class AddProductModel {

        [Required]
        [Display(Name = "Store URL")]
        public string url { get; set; }

        public AddProductModel() {
            
        }

        public override string ToString() {

            return JsonSerializer.Serialize<AddProductModel>(this);

        }

    }

}
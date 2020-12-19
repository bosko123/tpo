using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace web.Models {

    public class LoginModel {

        [EmailAddress]
        [Required]
        [Display(Name = "Email")]
        public string email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }

        public LoginModel() {

        }

        public override string ToString() {

            return JsonSerializer.Serialize<LoginModel>(this);

        }

    }

}
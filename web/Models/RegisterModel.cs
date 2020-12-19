using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace web.Models {

    public class RegisterModel {

        [Required]
        [Display(Name = "Name")]
        public string name { get; set; }
        [Required]
        [Display(Name = "Surname")]
        public string surname { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }
        [JsonIgnore]
        [Required]
        [Display(Name = "Confirm password")]
        public string confirmPassword { get; set; }
        [EmailAddress]
        [Required]
        [Display(Name = "Email")]
        public string email { get; set; }

        public RegisterModel() {

        }

        public override string ToString() {

            return JsonSerializer.Serialize<RegisterModel>(this);

        }

    }

}
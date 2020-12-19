using System.Text.Json;
using System.Text.Json.Serialization;

namespace web.Models {

    public class User {
        public string token { get; set; }
        public string name { get; set; }
        public string surname { get; set; }

        public User() {

        }

        public override string ToString() {

            return JsonSerializer.Serialize<User>(this);

        }

    }

}
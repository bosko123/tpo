using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace web.Models {

    public class SetThreshold {

        public int id { get; set; }
        public object spodnja { get; set; }
        public object zgornja { get; set; }

        public SetThreshold() {
            
        }

        public override string ToString() {

            return JsonSerializer.Serialize<SetThreshold>(this);

        }

    }

}

using System.ComponentModel.DataAnnotations;

namespace backend_staffdirectory.Models {
    public class AuthenticateRequest {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}

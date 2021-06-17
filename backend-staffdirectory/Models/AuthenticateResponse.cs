using backend_staffdirectory.Entities;

namespace backend_staffdirectory.Models {
    public class AuthenticateResponse {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Supervisor { get; set; }
        public string Position { get; set; }

        public AuthenticateResponse(User user, string token) {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            Token = token;
            Role = user.Role;
            Email = user.Email;
            Supervisor = user.Supervisor;
            Position = user.Position;
        }
    }
}

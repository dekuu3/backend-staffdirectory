using System.Text.Json.Serialization;

namespace backend_staffdirectory.Entities {
    public class User {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }

        public UserInfo UserInfo { get; set; }

        // [JsonIgnore] attribute prevents the password property from being serialized and returned in api responses
        [JsonIgnore]
        public string Password { get; set; }
    }

    public class UserInfo {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Supervisor { get; set; }
        public string Position { get; set; }
        public string Privileges { get; set; }

        public User User { get; set; }
    }
}

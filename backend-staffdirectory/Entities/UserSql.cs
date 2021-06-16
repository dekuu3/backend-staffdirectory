using System.Text.Json.Serialization;

/*
 * To parse sql queries more easily, otherwise need to make a bigger dapper statement 
 */

namespace backend_staffdirectory.Entities {
    public class UserSql {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Supervisor { get; set; }
        public string Position { get; set; }
    }
}

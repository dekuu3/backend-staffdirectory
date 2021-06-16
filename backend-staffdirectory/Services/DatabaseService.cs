using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using backend_staffdirectory.Entities;
using MySqlConnector;
using Dapper;

namespace backend_staffdirectory.Services {
    public interface IDatabaseService {
        List<User> GetAllUsers();
        User GetUserByUsernameAndPassword(string un, string pw);
        User GetUserById(int id);
        UserSql AddUser(UserSql user);
    }

    public class DatabaseService : IDatabaseService {
        private readonly IConfiguration _config;

        public DatabaseService() { }

        public DatabaseService(IConfiguration config) {
            _config = config;
        }

        public List<User> GetAllUsers() {
            var _conn = new MySqlConnection(_config["ConnectionString"]);

            string sql = "SELECT * FROM users";

            var users = _conn.Query<UserSql>(sql).ToList();

            List<User> newUserList = new();

            foreach (var u in users) {
                User newUser = new() {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Username = u.LastName,
                Password = u.Password,
                Role = u.Role,
                Email = u.Email,
                Supervisor = u.Supervisor,
                Position = u.Position,
            };
                newUserList.Add(newUser);
            }
            return newUserList;
        }

        public User GetUserByUsernameAndPassword(string un, string pw) {
            var _conn = new MySqlConnection(_config["ConnectionString"]);
            string sql = "SELECT * FROM users WHERE Username = @Username AND Password = @Password";

            var user = _conn.Query<UserSql>(sql, new { Username = un, Password = pw }).ToList();

            if (user.Count() == 0 || user == null) return null;

            User newUser = new();

            foreach (var u in user) {
                newUser.Id = u.Id;
                newUser.FirstName = u.FirstName;
                newUser.LastName = u.LastName;
                newUser.Username = u.LastName;
                newUser.Password = u.Password;
                newUser.Role = u.Role;
                newUser.Email = u.Email;
                newUser.Supervisor = u.Supervisor;
                newUser.Position = u.Position;
            }
            return newUser;
        }

        public User GetUserById(int id) {
            var _conn = new MySqlConnection(_config["ConnectionString"]);
            string sql = "SELECT * FROM users WHERE Id = @Id";

            var user = _conn.Query<UserSql>(sql, new { Id = id }).ToList();

            if (user.Count() == 0 || user == null) return null;

            User newUser = new();

            foreach (var u in user) {
                newUser.Id = u.Id;
                newUser.FirstName = u.FirstName;
                newUser.LastName = u.LastName;
                newUser.Username = u.LastName;
                newUser.Password = u.Password;
                newUser.Role = u.Role;
                newUser.Email = u.Email;
                newUser.Supervisor = u.Supervisor;
                newUser.Position = u.Position;
            }
            return newUser;
        }

        public UserSql AddUser(UserSql user) {
            var _conn = new MySqlConnection(_config["ConnectionString"]);
            string sql = "INSERT INTO users (FirstName, LastName, Username, Role, Password, Email, Supervisor, Position) VALUES (@FirstName, @LastName, @Username, @Role, @Password, @Email, @Supervisor, @Position)";

            var query = _conn.Execute(sql, new {
                user.FirstName,
                user.LastName,
                user.Username,
                user.Role,
                user.Password,
                user.Email,
                user.Supervisor,
                user.Position
            });

            return user;
        }
    }
}

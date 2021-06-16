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
    }

    public class DatabaseService : IDatabaseService {
        private readonly IConfiguration _config;

        public DatabaseService() {

        }

        public DatabaseService(IConfiguration config) {
            _config = config;
        }

        public List<User> GetAllUsers() {
            var _conn = new MySqlConnection(_config["ConnectionString"]);

            string test = "SELECT * FROM users INNER JOIN usersinfo ON users.Id = usersinfo.UserId";

            var users = _conn.Query<UserSql>(test).ToList();

            List<User> newUserList = new();

            foreach (var u in users) {
                User newUser = new() {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Username = u.LastName,
                    Password = u.Password,
                    Role = u.Role                    
                };

                UserInfo newUserInfo = new() {
                    Id = newUser.Id,
                    UserId = u.UserId,
                    Email = u.Email,
                    Supervisor = u.Supervisor,
                    Position = u.Position,
                };
                newUser.UserInfo = newUserInfo;
                newUserList.Add(newUser);
            }
            return newUserList;
        }

        public User GetUserByUsernameAndPassword(string un, string pw) {
            var _conn = new MySqlConnection(_config["ConnectionString"]);
            string sqlUser = "SELECT * FROM users INNER JOIN usersinfo ON users.Id = usersinfo.UserId WHERE users.Username = @Username AND users.Password = @Password";

            var user = _conn.Query<UserSql>(sqlUser, new { Username = un, Password = pw }).ToList();

            if (user.Count() == 0 || user == null) return null;

            User newUser = new();

            foreach (var u in user) {
                newUser.Id = u.Id;
                newUser.FirstName = u.FirstName;
                newUser.LastName = u.LastName;
                newUser.Username = u.LastName;
                newUser.Password = u.Password;
                newUser.Role = u.Role;
                
                UserInfo newUserInfo = new() {
                    Id = newUser.Id,
                    UserId = u.UserId,
                    Email = u.Email,
                    Supervisor = u.Supervisor,
                    Position = u.Position,
                };
                newUser.UserInfo = newUserInfo;
            }
            return newUser;
        }

        public User GetUserById(int id) {
            var _conn = new MySqlConnection(_config["ConnectionString"]);
            string sqlUser = "SELECT * FROM users JOIN usersinfo ON users.Id = usersinfo.UserId WHERE users.Id = @Id";

            var user = _conn.Query<UserSql>(sqlUser, new { Id = id }).ToList();

            if (user.Count() == 0 || user == null) return null;

            User newUser = new();

            foreach (var u in user) {
                newUser.Id = u.Id;
                newUser.FirstName = u.FirstName;
                newUser.LastName = u.LastName;
                newUser.Username = u.LastName;
                newUser.Password = u.Password;
                newUser.Role = u.Role;

                UserInfo newUserInfo = new() {
                    Id = newUser.Id,
                    UserId = u.UserId,
                    Email = u.Email,
                    Supervisor = u.Supervisor,
                    Position = u.Position,
                };
                newUser.UserInfo = newUserInfo;
            }
            return newUser;
        }
    }
}

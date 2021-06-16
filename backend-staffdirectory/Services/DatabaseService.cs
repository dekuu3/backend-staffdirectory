using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using backend_staffdirectory.Entities;
using MySqlConnector;
using Dapper;

namespace backend_staffdirectory.Services {
    public interface IDatabaseService {
        List<User> GetAllUsers();
        List<User> GetUserByUsernameAndPassword(string un, string pw);
        List<User> GetUserById(int id);
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

            var allUsers = _conn.Query<UserSql>(test).ToList();

            List<User> newUserList = new();

            foreach (var user in allUsers) {
                User newUser = new() {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.LastName,
                    Password = user.Password,
                    Role = user.Role                    
                };

                UserInfo newUserInfo = new() {
                    Id = newUser.Id,
                    UserId = user.UserId,
                    Email = user.Email,
                    Supervisor = user.Supervisor,
                    Position = user.Position,
                };
                newUser.UserInfo = newUserInfo;
                newUserList.Add(newUser);
            }
            return newUserList;
        }

        public List<User> GetUserByUsernameAndPassword(string un, string pw) {
            var _conn = new MySqlConnection(_config["ConnectionString"]);
            string sqlUser = "SELECT * FROM users INNER JOIN usersinfo ON users.Id = usersinfo.UserId WHERE users.Username = @Username AND users.Password = @Password";

            var allUsers = _conn.Query<UserSql>(sqlUser, new { Username = un, Password = pw }).ToList();

            if (allUsers == null) return null;

            List<User> newUserList = new();

            foreach (var user in allUsers) {
                User newUser = new() {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.LastName,
                    Password = user.Password,
                    Role = user.Role
                };

                UserInfo newUserInfo = new() {
                    Id = newUser.Id,
                    UserId = user.UserId,
                    Email = user.Email,
                    Supervisor = user.Supervisor,
                    Position = user.Position,
                };
                newUser.UserInfo = newUserInfo;
                newUserList.Add(newUser);
            }
            return newUserList;
        }

        public List<User> GetUserById(int id) {
            var _conn = new MySqlConnection(_config["ConnectionString"]);
            string sqlUser = "SELECT * FROM users JOIN usersinfo ON users.Id = usersinfo.UserId WHERE users.Id = @Id";

            var allUsers = _conn.Query<UserSql>(sqlUser, new { Id = id }).ToList();

            if (allUsers == null) return null;

            List<User> newUserList = new();
            foreach (var user in allUsers) {
                User newUser = new() {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.LastName,
                    Password = user.Password,
                    Role = user.Role
                };

                UserInfo newUserInfo = new() {
                    Id = newUser.Id,
                    UserId = user.UserId,
                    Email = user.Email,
                    Supervisor = user.Supervisor,
                    Position = user.Position,
                };
                newUser.UserInfo = newUserInfo;
                newUserList.Add(newUser);
            }
            return newUserList;
        }
    }
}

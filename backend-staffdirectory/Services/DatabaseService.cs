using Microsoft.Extensions.Configuration;
using backend_staffdirectory.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend_staffdirectory.Entities;
using MySqlConnector;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace backend_staffdirectory.Services {
    public interface IDatabaseService {
        //methods in DatabaseService class
        List<User> GetAllUsers();

        List<User> GetUserByUsernameAndPassword(string un);

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
            //string sqlAllUsers = "SELECT * FROM users";
            string test = "SELECT users.FirstName, users.LastName, users.Username, usersinfo.Position FROM users INNER JOIN usersinfo ON users.Id = usersinfo.UserId";

            var allUsers = _conn.Query<User>(test).ToList();

            return allUsers;
        }

        public List<User> GetUserByUsernameAndPassword(string un) {
            var _conn = new MySqlConnection(_config["ConnectionString"]);
            string sqlUser = "SELECT * FROM users WHERE users.Username = @Username";

            var user = _conn.Query<User>(sqlUser, new { Username = un }).ToList();

            return user;
        }

        public List<User> GetUserById(int id) {
            var _conn = new MySqlConnection(_config["ConnectionString"]);
            string sqlUser = "SELECT * FROM users WHERE users.Id = @Id";

            var user = _conn.Query<User>(sqlUser, new { Id = id }).ToList();

            return user;
        }
    }
}

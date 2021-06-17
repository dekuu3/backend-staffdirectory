using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using backend_staffdirectory.Entities;
using MySqlConnector;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;

namespace backend_staffdirectory.Services {
    public interface IDatabaseService {
        List<User> GetAllUsers();
        User GetUserByUsernameAndPassword(string un, string pw);
        User GetUserById(int id);
        User EditUserById(int id, User user);
        User EditProfileById(int id, UserSql user);
        int DeleteUserById(int id);
        int AddUser(UserSql user);
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

        public User EditUserById(int id, User user) {
            var _conn = new MySqlConnection(_config["ConnectionString"]);

            string pwSql = "SELECT Id, Password FROM users WHERE Id = @Id";
            var pwQuery = _conn.Query<User>(pwSql, new { Id = id }).ToList();

            var password = pwQuery.First().Password;
            var idDb = pwQuery.First().Id;

            string sql = "UPDATE users SET Id = @Id, FirstName = @FirstName, LastName = @LastName, Username = @Username, Role = @Role, Password = @Password, Email = @Email, Supervisor = @Supervisor, Position = @Position WHERE Id = @Id";

            try {
                var query = _conn.Execute(sql, new {
                    idDb,
                    user.FirstName,
                    user.LastName,
                    user.Username,
                    user.Role,
                    password,
                    user.Email,
                    user.Supervisor,
                    user.Position,
                    Id = id
                });
            }
            catch (DbUpdateException) {
                throw;
            }

            return user;
        }

        public User EditProfileById(int id, UserSql user) {
            var _conn = new MySqlConnection(_config["ConnectionString"]);

            string pwSql = "SELECT Id FROM users WHERE Id = @Id";
            var pwQuery = _conn.Query<User>(pwSql, new { Id = id }).ToList();

            var idDb = pwQuery.First().Id;

            string sql = "UPDATE users SET Id = @Id, FirstName = @FirstName, LastName = @LastName, Username = @Username, Role = @Role, Password = @Password, Email = @Email, Supervisor = @Supervisor, Position = @Position WHERE Id = @Id";

            try {
                var query = _conn.Execute(sql, new {
                    idDb,
                    user.FirstName,
                    user.LastName,
                    user.Username,
                    user.Role,
                    user.Password,
                    user.Email,
                    user.Supervisor,
                    user.Position,
                    Id = id
                });
            }
            catch (DbUpdateException) {
                throw;
            }

            User newUser = new() {
                Id = idDb,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Password = user.Password,
                Email = user.Email,
                Supervisor = user.Supervisor,
                Position = user.Position
            };

            return newUser;
        }

        public int DeleteUserById(int id) {
            var _conn = new MySqlConnection(_config["ConnectionString"]);
            string sql = "DELETE FROM users WHERE Id = @Id";

            var rowsAffected = _conn.Execute(sql, new { id });
            return rowsAffected;
        }

        public int AddUser(UserSql user) {
            var checkIfUserExists = GetUserByUsernameAndEmail(user);
            
            if (checkIfUserExists == true) {
                return 0;
            } else {
                var _conn = new MySqlConnection(_config["ConnectionString"]);
                string sql = "INSERT INTO users (FirstName, LastName, Username, Role, Password, Email, Supervisor, Position) VALUES (@FirstName, @LastName, @Username, @Role, @Password, @Email, @Supervisor, @Position);";
                int query;
                try {
                    query = _conn.Execute(sql, new {
                        user.FirstName,
                        user.LastName,
                        user.Username,
                        user.Role,
                        user.Password,
                        user.Email,
                        user.Supervisor,
                        user.Position
                    });

                    return query;
                }
                catch (Exception) {
                    return 0;
                }
            }
        }

        public bool GetUserByUsernameAndEmail(UserSql user) {
            var _conn = new MySqlConnection(_config["ConnectionString"]);
            string sql = "SELECT * FROM users WHERE Username = @Username AND Email = @Email";

            var response = _conn.Query<UserSql>(sql, new {
                user.Username,
                user.Email,
            });

            if (response.Count() == 0) {
                return false;
            }

            return true;
        }
    }
}

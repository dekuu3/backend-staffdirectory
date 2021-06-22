using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using backend_staffdirectory.Entities;
using MySqlConnector;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;

namespace backend_staffdirectory.Services {
    public interface IDatabaseService {
        List<User> GetAllUsers();
        UserSql GetUserByUsername(string un);
        User GetUserById(int id);
        User EditUserById(int id, User user);
        User EditProfileById(int id, UserSql user);
        int DeleteUserById(int id);
        int AddUser(UserSql user);
        bool EditProfilePhotoById(string url, int id);
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

            var users = _conn.Query<User>(sql).ToList();

            return users;
        }

        public UserSql GetUserByUsername(string un) {
            var _conn = new MySqlConnection(_config["ConnectionString"]);
            string sql = "SELECT * FROM users WHERE Username = @Username";

            var user = _conn.Query<UserSql>(sql, new { Username = un}).ToList();

            if (user.Count() == 0 || user == null) return null;

            return user.First();
        }

        public User GetUserById(int id) {
            var _conn = new MySqlConnection(_config["ConnectionString"]);
            string sql = "SELECT * FROM users WHERE Id = @Id";

            var user = _conn.Query<User>(sql, new { Id = id }).ToList();

            if (user.Count() == 0 || user == null) return null;

            return user.First();
        }

        public User EditUserById(int id, User user) {
            var _conn = new MySqlConnection(_config["ConnectionString"]);

            string pwSql = "SELECT * FROM users WHERE Id = @Id";
            var query = _conn.Query<User>(pwSql, new { Id = id }).ToList();

            var idInDb = query.First().Id;
            var passwordInDb = query.First().Password;

            var tempUser = PopulateEditUserQuery(query, user);

            string sql = "UPDATE users SET Id = @Id, FirstName = @FirstName, LastName = @LastName, Username = @Username, Role = @Role, Password = @Password, Email = @Email, Supervisor = @Supervisor, Position = @Position WHERE Id = @Id";

            try {
                _conn.Execute(sql, new {
                    idInDb,
                    tempUser.FirstName,
                    tempUser.LastName,
                    tempUser.Username,
                    tempUser.Role,
                    Password = passwordInDb,
                    tempUser.Email,
                    tempUser.Supervisor,
                    tempUser.Position,
                    Id = id
                });
            }
            catch (Exception ex) {
                Debug.WriteLine(ex);
                throw;
            }

            return tempUser;
        }

        public User EditProfileById(int id, UserSql user) {
            var _conn = new MySqlConnection(_config["ConnectionString"]);

            string pwSql = "SELECT * FROM users WHERE Id = @Id";
            var query = _conn.Query<User>(pwSql, new { Id = id }).ToList();

            var idInDb = query.First().Id;

            var tempUser = PopulateEditProfileQuery(query, user);

            string sql = "UPDATE users SET Id = @Id, FirstName = @FirstName, LastName = @LastName, Username = @Username, Role = @Role, Password = @Password, Email = @Email, Supervisor = @Supervisor, Position = @Position WHERE Id = @Id";

            try {
                _conn.Execute(sql, new {
                    idInDb,
                    tempUser.FirstName,
                    tempUser.LastName,
                    tempUser.Username,
                    tempUser.Role,
                    tempUser.Password,
                    tempUser.Email,
                    tempUser.Supervisor,
                    tempUser.Position,
                    Id = id
                });
            }
            catch (Exception ex) {
                Debug.WriteLine(ex);
                throw;
            }

            User newUser = new() {
                Id = idInDb,
                FirstName = tempUser.FirstName,
                LastName = tempUser.LastName,
                Username = tempUser.Username,
                Role = tempUser.Role,
                Email = tempUser.Email,
                Supervisor = tempUser.Supervisor,
                Position = tempUser.Position,
                Image = tempUser.Image
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
                catch (Exception ex) {
                    Debug.WriteLine("Exception " + ex);
                    return 0;
                }
            }
        }

        public bool EditProfilePhotoById(string url, int id) {
            var _conn = new MySqlConnection(_config["ConnectionString"]);

            string pwSql = "SELECT * FROM users WHERE Id = @Id";
            var query = _conn.Query<User>(pwSql, new { Id = id }).ToList();

            var idInDb = query.First().Id;

            string sql = "UPDATE users SET Image = @Image WHERE Id = @Id";

            try {
                _conn.Execute(sql, new {
                    Image = url,
                    Id = id
                });
            }
            catch (Exception ex) {
                Debug.WriteLine(ex);
                return false;
            }

            return true;
        }

        // HELPER FUNCTIONS
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

        // If some values are null they're replaced by the existing values in the DB
        public User PopulateEditUserQuery(List<User> dbUser, User user) {
            User tempUser = new();

            tempUser.Id = dbUser.First().Id;

            tempUser.FirstName = (user.FirstName == null || user.FirstName.Trim() == "") ? dbUser.First().FirstName : user.FirstName;

            tempUser.LastName = (user.LastName == null || user.LastName.Trim() == "") ? dbUser.First().LastName : user.LastName;

            tempUser.Username = (user.Username == null || user.Username.Trim() == "") ? dbUser.First().Username : user.Username;

            tempUser.Role = (user.Role == null || user.Role.Trim() == "") ? dbUser.First().Role : user.Role;

            tempUser.Email = (user.Email == null || user.Email.Trim() == "") ? dbUser.First().Email : user.Email;

            tempUser.Supervisor = (user.Supervisor == null || user.Supervisor.Trim() == "") ? dbUser.First().Supervisor : user.Supervisor;

            tempUser.Position = (user.Position == null || user.Position.Trim() == "") ? dbUser.First().Position : user.Position;

            tempUser.Image = dbUser.First().Image;

            return tempUser;
        }

        public UserSql PopulateEditProfileQuery(List<User> dbUser, UserSql user) {
            UserSql tempUser = new();

            tempUser.Id = dbUser.First().Id;

            tempUser.FirstName = (user.FirstName == null || user.FirstName.Trim() == "") ? dbUser.First().FirstName : user.FirstName;

            tempUser.LastName = (user.LastName == null || user.LastName.Trim() == "") ? dbUser.First().LastName : user.LastName;

            tempUser.Username = (user.Username == null || user.Username.Trim() == "") ? dbUser.First().Username : user.Username;

            tempUser.Role = (user.Role == null || user.Role.Trim() == "") ? dbUser.First().Role : user.Role;

            tempUser.Password = (user.Password == null || user.Password.Trim() == "") ? dbUser.First().Password : user.Password;

            tempUser.Email = (user.Email == null || user.Email.Trim() == "") ? dbUser.First().Email : user.Email;

            tempUser.Supervisor = (user.Supervisor == null || user.Supervisor.Trim() == "") ? dbUser.First().Supervisor : user.Supervisor;

            tempUser.Position = (user.Position == null || user.Position.Trim() == "" ) ? dbUser.First().Position : user.Position;

            tempUser.Image = dbUser.First().Image;

            return tempUser;
        }
    }
}

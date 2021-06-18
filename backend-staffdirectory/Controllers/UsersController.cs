using Microsoft.AspNetCore.Mvc;
using backend_staffdirectory.Models;
using backend_staffdirectory.Services;
using backend_staffdirectory.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace backend_staffdirectory.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase {
        private readonly IUserService _userService;
        private readonly IDatabaseService _dbService;

        public UsersController(IUserService userService, IDatabaseService databaseService) {
            _userService = userService;
            _dbService = databaseService;
        }

        // To log in
        // POST : /users/authenticate
        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model) {
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        // Gets all users
        // GET : /users
        [Authorize]
        [HttpGet]
        public IActionResult GetAllUsers() {
            var response = _dbService.GetAllUsers();

            if (response == null) {
                return BadRequest(new { message = "No users found" });
            }

            return Ok(response);
        }

        // Gets user by id
        // GET : /users/{id}
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id) {
            var response = _dbService.GetUserById(id);

            if (response == null) {
                return BadRequest(new { message = "User not found" });
            }

            return Ok(response);
        }

        // Edits user data by id
        // PUT : /users/{id}/edit
        [AuthorizeAdmin]
        [HttpPut("{id}/edit")]
        public IActionResult EditUser(int id, User user) {
            var response = _dbService.EditUserById(id, user);

            if (response == null) {
                return BadRequest(new { message = "User not found" });
            }

            return Ok(response);
        }

        // Deletes user
        // DELETE : /users/{id}/delete
        [AuthorizeAdmin]
        [HttpDelete("{id}/delete")]
        public IActionResult DeleteUser(int id) {
            var response = _dbService.DeleteUserById(id);

            return Ok("User Id " + id + " Successfully Deleted! " + response + " Row Deleted.");
        }

        // Adds user
        // POST : /users/adduser
        [AuthorizeAdmin]
        [HttpPost("adduser")]
        public IActionResult AddUser(UserSql user) {
            user.Password = _userService.Hash(user.Password);
            var response = _dbService.AddUser(user);

            if (response == 0) {
                return BadRequest(new { message = "An error occurred - Duplicate entry (Check username or email)" });
            }

            // number of users added (should be 1)
            return Ok(response);
        }

        // Gets the profile of whoever is logged in
        // GET : /users/myprofile
        [Authorize]
        [HttpGet("myprofile")]
        public IActionResult GetProfile() {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var id = _userService.GetIdInToken(token);
            var response = _dbService.GetUserById(id);

            if (response == null) {
                return BadRequest(new { message = "User not found" });
            }

            return Ok(response);
        }

        // Edits user data of whoever's logged in
        // PUT : /users/myprofile/edit
        [Authorize]
        [HttpPut("myprofile/edit")]
        public IActionResult EditProfile(UserSql user) {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var id = _userService.GetIdInToken(token);

            if (user.Password != null || user.Password.Trim() != "") {
                user.Password = _userService.Hash(user.Password);
            }

            var response = _dbService.EditProfileById(id, user);

            if (response == null) {
                return BadRequest(new { message = "User not found" });
            }

            return Ok(response);
        }
    }
}

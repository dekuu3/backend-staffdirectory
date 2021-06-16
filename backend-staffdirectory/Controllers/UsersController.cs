using Microsoft.AspNetCore.Mvc;
using backend_staffdirectory.Models;
using backend_staffdirectory.Services;
using backend_staffdirectory.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;

/*
 * This controls the routes and http requests received from the front-end.
 * Makes use of the UserService to authenticate and retrieve users ( via the IUserService interface)
 */

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

        // POST : /users/authenticate
        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model) {
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        // GET : /users
        [Authorize]
        [HttpGet]
        public IActionResult GetUsers() {
            var response = _dbService.GetAllUsers();

            if (response == null) {
                return BadRequest(new { message = "No users found" });
            }

            return Ok(response);
        }

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

        // POST : /users/adduser
        [AuthorizeAdmin]
        [HttpPost("adduser")]
        public IActionResult AddUser(UserSql user) {
            var response = _dbService.AddUser(user);

            if (response == null) {
                return BadRequest(new { message = "An error occurred" });
            }

            return Ok(response);
        }

        // GET : /users/myprofile
        // Shows the profile of whoever is logged in
        [Authorize]
        [HttpGet("myprofile")]
        public IActionResult getProfile() {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var id = _userService.GetIdInContext(token);

            var response = _dbService.GetUserById(id);

            if (response == null) {
                return BadRequest(new { message = "User not found" });
            }

            return Ok(response);
        }
    }
}

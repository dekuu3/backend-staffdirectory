using Microsoft.AspNetCore.Mvc;
using backend_staffdirectory.Models;
using backend_staffdirectory.Services;
using backend_staffdirectory.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

/*
 * This controls the routes and http requests received from the front-end.
 * Makes use of the UserService to authenticate and retrieve users ( via the IUserService interface)
 */

namespace backend_staffdirectory.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase {
        private IUserService _userService;
        private IDatabaseService _dbService;

        public UsersController(IUserService userService, IDatabaseService databaseService) {
            _userService = userService;
            _dbService = databaseService;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model) {
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [AuthorizeAdmin]
        [HttpGet]
        public List<User> GetUsers() {
            return _dbService.GetAllUsers();
        }
    }
}

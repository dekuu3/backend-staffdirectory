/*
 * This controls the routes and http requests received from the front-end.
 * Makes use of the UserService to authenticate and retrieve users ( via the IUserService interface)
 */

using Microsoft.AspNetCore.Mvc;
using backend_staffdirectory.Models;
using backend_staffdirectory.Services;
using backend_staffdirectory.Entities;

namespace backend_staffdirectory.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase {
        private IUserService _userService;

        public UsersController(IUserService userService) {
            _userService = userService;
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
        public IActionResult GetAll() {
            var users = _userService.GetAll();
            return Ok(users);
        }
    }
}

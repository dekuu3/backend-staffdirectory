using Microsoft.AspNetCore.Mvc;
using backend_staffdirectory.Models;
using backend_staffdirectory.Services;
using backend_staffdirectory.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading;

namespace backend_staffdirectory.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase {
        private readonly IUserService _userService;
        private readonly IDatabaseService _dbService;
        private readonly ICloudinaryService _cloudinaryService;

        public UsersController(IUserService userService, IDatabaseService databaseService, ICloudinaryService cloudinaryService) {
            _userService = userService;
            _dbService = databaseService;
            _cloudinaryService = cloudinaryService;
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

            // Assign default image
            if (user.Image == null ) {
                user.Image = "https://res.cloudinary.com/dqmoirffd/image/upload/v1624287823/sample.jpg";
            }

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

            if (user.Password != null) {
                if (user.Password.Trim() != "") {
                    user.Password = _userService.Hash(user.Password);
                }
            };

            var response = _dbService.EditProfileById(id, user);

            if (response == null) {
                return BadRequest(new { message = "User not found" });
            }

            return Ok(response);
        }

        // Edits user image of whoever's logged in
        // POST : /users/myprofile/edit/image
        [Authorize]
        [HttpPost("myprofile/edit/image")]
        public IActionResult EditProfileImage([FromForm] IFormFile file) {         
            if (file == null) {
                return BadRequest(new { message = "Please upload a photo" });
            }
           
            // Get id of person making request
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var id = _userService.GetIdInToken(token);

            // Save temporary file to TempMedia folder
            _userService.WriteFile(file);

            // Upload file to cloudinary service
            var response = _cloudinaryService.UploadPhoto(file);

            if (response == null) {
                return BadRequest(new { message = "Error uploading photo" });
            }

            // Delete file from TempMedia folder
            _userService.DeleteFile(file);

            // Get url from ImageUploadResult object
            var url = _userService.ModifyImageUrl(response.SecureUrl.ToString());

            //add url to the db id
            var isSuccess = _dbService.EditProfilePhotoById(url, id);

            if (isSuccess == true) {
                return Ok((new { url }));
            }

            return BadRequest(new { message = "Error uploading photo" });
        }
    }
}

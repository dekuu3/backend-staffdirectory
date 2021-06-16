/*
 * This service:
 *      -authenticates users
 *      -retrieves all users
 *      -gets users by id
 *      -generates jwt tokens for users
 */

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using backend_staffdirectory.Models;
using backend_staffdirectory.Entities;
using backend_staffdirectory.Helpers;

namespace backend_staffdirectory.Services {
    public interface IUserService {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
    }

    public class UserService : IUserService {

        private readonly AppSettings _appSettings;
        private readonly IDatabaseService _dbService;

        public UserService(IOptions<AppSettings> appSettings, IDatabaseService dbService) {
            _appSettings = appSettings.Value;
            _dbService = dbService;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model) {
            var user = _dbService.GetUserByUsernameAndPassword(model.Username, model.Password);

            if (user == null) return null;

            // authentication successful so generate jwt token
            var token = generateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

        private string generateJwtToken(User user) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[] { 
                    new Claim("id", user.Id.ToString()),
                    //new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

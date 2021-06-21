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
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.IO;

/*
 * This service:
 *      -authenticates users
 *      -retrieves all users
 *      -gets users by id
 *      -generates jwt tokens for users
 *      -hashes passwords
 *      -verifies hashed passwords against passwords input by users
 */

namespace backend_staffdirectory.Services {
    public interface IUserService {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        int GetIdInToken(string token);
        string Hash(string password);
        bool VerifyPassword(string password, string hashedPassword);
        Task<bool> WriteFile(IFormFile file);
        bool DeleteFile(IFormFile file);
    }

    public class UserService : IUserService {

        private readonly AppSettings _appSettings;
        private readonly IDatabaseService _dbService;
        private readonly IConfiguration _config;

        public UserService(IOptions<AppSettings> appSettings, IDatabaseService dbService, IConfiguration config) {
            _appSettings = appSettings.Value;
            _dbService = dbService;
            _config = config;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model) {
            var user = _dbService.GetUserByUsername(model.Username);
            
            if (user == null) return null;

            var isPasswordCorrect = VerifyPassword(model.Password, user.Password);

            if (isPasswordCorrect == false) return null;

            User newUser = new() {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Role = user.Role,
                Email = user.Email,
                Supervisor = user.Supervisor,
                Position = user.Position
            };

            // authentication successful so generate jwt token
            var token = generateJwtToken(newUser);

            return new AuthenticateResponse(newUser, token);
        }

        public int GetIdInToken(string token) {
            return ReadToken(token);
        }

        // Creates a hash from a password with 10000 iterations
        public string Hash(string password) {
            return Hash(password, 10000);
        }

        // Verifies a password against a hash.
        public bool VerifyPassword(string password, string hashedPassword) {
            return Verify(password, hashedPassword);
        }

        // Saves file to TempMedia folder
        public async Task<bool> WriteFile(IFormFile file) {

            try {
                var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "TempMedia");

                if (!Directory.Exists(pathBuilt)) {
                    Directory.CreateDirectory(pathBuilt);
                }

                var path = Path.Combine(Directory.GetCurrentDirectory(), "TempMedia", file.FileName);

                using (var stream = new FileStream(path, FileMode.Create)) {
                    await file.CopyToAsync(stream);
                }

                return true;
            } 
            catch (Exception) {
                return false;
            }
        }

        // Deletes file from TempMedia folder
        public bool DeleteFile(IFormFile file) {
            try {
                File.Delete($@"TempMedia/{file.FileName}");
                
                return true;
            }
            catch (Exception) {
                return false;
            }
        }

        // HELPER FUNCTIONS
        private int ReadToken(string token) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            tokenHandler.ValidateToken(token, new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            return userId;
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

        // Creates a hash from a password.
        private string Hash(string password, int iterations) {
            var SaltSize = int.Parse(_config["SaltSize"]);
            var HashSize = int.Parse(_config["HashSize"]);
            // Create salt
            using (var rng = new RNGCryptoServiceProvider()) {
                byte[] salt;
                rng.GetBytes(salt = new byte[SaltSize]);
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations)) {
                    var hash = pbkdf2.GetBytes(HashSize);
                    // Combine salt and hash
                    var hashBytes = new byte[SaltSize + HashSize];
                    Array.Copy(salt, 0, hashBytes, 0, SaltSize);
                    Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);
                    // Convert to base64
                    var base64Hash = Convert.ToBase64String(hashBytes);

                    // Format hash with extra information
                    return $"$HASH|V1${iterations}${base64Hash}";
                }
            }
        }

        // Checks if hash is supported.
        private static bool IsHashSupported(string hashString) {
            return hashString.Contains("HASH|V1$");
        }

        // Verifies a password against a hash.
        private bool Verify(string password, string hashedPassword) {
            var SaltSize = int.Parse(_config["SaltSize"]);
            var HashSize = int.Parse(_config["HashSize"]);

            // Check hash
            if (!IsHashSupported(hashedPassword)) {
                throw new NotSupportedException("The hashtype is not supported");
            }

            // Extract iteration and Base64 string
            var splittedHashString = hashedPassword.Replace("$HASH|V1$", "").Split('$');
            var iterations = int.Parse(splittedHashString[0]);
            var base64Hash = splittedHashString[1];

            // Get hash bytes
            var hashBytes = Convert.FromBase64String(base64Hash);

            // Get salt
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Create hash with given salt
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations)) {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                // Get result
                for (var i = 0; i < HashSize; i++) {
                    if (hashBytes[i + SaltSize] != hash[i]) {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}

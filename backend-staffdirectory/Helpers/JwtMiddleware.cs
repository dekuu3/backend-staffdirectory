using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backend_staffdirectory.Services;

/*
 * This middleware checks if there is a token in the request "Authorization" header
 *      If so:
 *      - validates token
 *      - extracts user id from token
 *      - attaches authenticated user  and role to the current httpcontext.items collection
 *          to make it accessible within the scope of the current request
 * If theres no token or any of these steps fail then no user is attached to the httpcontext and
 * and the request is only able to access the public routes
 */

namespace backend_staffdirectory.Helpers {
    public class JwtMiddleware {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;
        private readonly IDatabaseService _databaseService;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings, IDatabaseService databaseService) {
            _next = next;
            _appSettings = appSettings.Value;
            _databaseService = databaseService;
        }

        public async Task Invoke(HttpContext context, IUserService userService) {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                attachUserToContext(context, userService, token);

            await _next(context);
        }

        private void attachUserToContext(HttpContext context, IUserService userService, string token) {
            try {
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
                //var userRole = jwtToken.Claims.First(x => x.Type == "Role").GetType();

                // attach user to context on successful jwt validation
                context.Items["User"] = _databaseService.GetUserById(userId);

                // attach role to context on successful jwt validation
                //context.Items["Role"] = userService.GetById(userId).Role;
            }
            catch {
                // jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}

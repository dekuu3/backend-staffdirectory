/*
 * Custom attribute ensures only "Admin"'s are authorized to access a particular api endpoint
 */

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using backend_staffdirectory.Entities;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAdminAttribute : Attribute, IAuthorizationFilter {
    public void OnAuthorization(AuthorizationFilterContext context) {
        var user = (User)context.HttpContext.Items["User"];
        if (user == null) {
            // not logged in
            context.Result = new JsonResult(new { message = "You're not logged in" }) { StatusCode = StatusCodes.Status401Unauthorized };
            return;
        }

        if (user.Role != "Admin") {
            // not authorized to access this
            context.Result = new JsonResult(new { message = "You're not authorized" }) { StatusCode = StatusCodes.Status403Forbidden };
        }

        // My attempt at using claims
        // http endpoints with the [Authorize] attribute are only allowing 
        // logged in users with the Admin role
        //var role = (User)context.HttpContext.Items["Role"];
    }
}
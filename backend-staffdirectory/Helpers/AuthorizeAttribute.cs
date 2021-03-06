/*
 * Custom attribute ensures any logged in "User" role is authorized to access a particular api endpoint
 */

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using backend_staffdirectory.Entities;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter {
    public void OnAuthorization(AuthorizationFilterContext context) {
        var user = (User)context.HttpContext.Items["User"];
        if (user == null) {
            // not logged in
            context.Result = new JsonResult(new { message = "You're not logged in" }) { StatusCode = StatusCodes.Status401Unauthorized };
            return;
        }
    }
}
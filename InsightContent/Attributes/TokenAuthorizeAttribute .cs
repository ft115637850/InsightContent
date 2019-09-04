using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsightContent.Attributes
{
    public class TokenAuthorizeAttribute: AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string controllerName = context.ActionDescriptor.RouteValues["controller"];
            string actionName = context.ActionDescriptor.RouteValues["action"];
            var bearerToken = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            // TO DO:
            //string roles = GetRoles.GetActionRoles(actionName, controllerName);
            //if (!string.IsNullOrWhiteSpace(roles))
            //{
            //    this.Roles = roles;
            //}
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using AuthenticationService.Managers;
using AuthenticationService.Models;
using InsightContent.Services;
using Microsoft.AspNetCore.Mvc;

namespace InsightContent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IUserService userSvc;

        public TokenController(IAuthService jwtSvc, IUserService userSvc)
        {
            this.authService = jwtSvc;
            this.userSvc = userSvc;
        }

        // GET: api/Token
        [HttpGet]
        public ActionResult<string> Get()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return StatusCode((int)HttpStatusCode.Forbidden, "Invalid Authorization Header");

            Claim[] claims;
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                if (authHeader.Scheme == "Bearer")
                {
                    claims = this.authService.GetTokenClaims(authHeader.Parameter).ToArray<Claim>();
                }
                else
                {
                    var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                    var username = credentials[0];
                    var password = credentials[1];
                    claims = userSvc.Authenticate(username, password);
                    if (claims == null)
                    {
                        return StatusCode((int)HttpStatusCode.NotFound, "User not found");
                    }
                }
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Invalid Authorization Header");
            }
            
            var token = this.authService.GenerateToken(new JWTContainerModel
            {
                Claims = claims
            });
            return Ok(token);
        }
    }
}
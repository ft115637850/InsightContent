using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthenticationService.Managers;
using AuthenticationService.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InsightContent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IAuthService authService;

        public TokenController(IAuthService jwtSvc)
        {
            this.authService = jwtSvc;
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
                    // user = await _userService.Authenticate(username, password);
                    claims = new Claim[] {
                        new Claim(ClaimTypes.Name, "user1"),
                        new Claim(ClaimTypes.Email, "user1@a.com"),
                        new Claim(ClaimTypes.Role, "user")
                    };
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
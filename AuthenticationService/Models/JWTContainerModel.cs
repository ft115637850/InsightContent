using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace AuthenticationService.Models
{
    class JWTContainerModel : IAuthContainerModel
    {
        public string SecretKey { get; set; }
        public string SecurityAlgorithm { get; set; }
        public int ExpireMinutes { get; set; }
        public Claim[] Claims { get; set; }
    }
}

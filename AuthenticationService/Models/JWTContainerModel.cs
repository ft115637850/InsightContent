using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationService.Models
{
    public class JWTContainerModel : IAuthContainerModel
    {
        public string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256;
        public int ExpireMinutes { get; set; } = 10080;
        public Claim[] Claims { get; set; }
    }
}

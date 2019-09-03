using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace AuthenticationService.Models
{
    public interface IAuthContainerModel
    {
        string SecurityAlgorithm { get; set; }
        int ExpireMinutes { get; set; }
        Claim[] Claims { get; set; }
    }
}

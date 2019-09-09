using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InsightContent.Services
{
    public interface IUserService
    {
        Claim[] Authenticate(string username, string password);
    }
}

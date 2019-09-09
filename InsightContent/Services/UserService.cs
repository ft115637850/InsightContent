using DataBaseAccessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InsightContent.Services
{
    public class UserService : IUserService
    {
        private readonly IDBAccessService dbAccess;
        
        public UserService(IDBAccessService dbAccess)
        {
            this.dbAccess = dbAccess;
        }

        public Claim[] Authenticate(string username, string password)
        {
            var parms = new Tuple<string, object>[] {
                new Tuple<string, object>("@name", username),
                new Tuple<string, object>("@password", password),
                new Tuple<string, object>("@key", this.dbAccess.EncryptKey),
            };

            var result = this.dbAccess.GetData("select id, name, email, roles from user where name=@name and password=aes_encrypt(@password,@key)", parms);
            if (result.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                var roles = Convert.ToString(result).Split(',');
                var claims = new Claim[roles.Length+3];
                claims[0] = new Claim(ClaimTypes.Name, username);
                claims[1] = new Claim(ClaimTypes.PrimarySid, Convert.ToString(result.Rows[0]["id"]));
                claims[2] = new Claim(ClaimTypes.Email, Convert.ToString(result.Rows[0]["email"]));
                int i = 3;
                foreach(var role in roles)
                {
                    claims[i++] = new Claim(ClaimTypes.Role, role);
                }
                return claims;
            }
            
        }
    }
}

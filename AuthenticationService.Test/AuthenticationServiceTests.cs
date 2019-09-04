using AuthenticationService.Managers;
using AuthenticationService.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;
using System.Linq;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationService.Test
{
    [TestClass]
    public class AuthenticationServiceTests
    {
        #region members
        private string testSecret = "A7BAA978-5DA5-487E-B507-1FFC9C05BF55";

        private string testSecret2 = "B7BAA978-5DA5-487E-B507-1FFC9C05BF55";
        #endregion

        [TestMethod]
        public void TestGenerateToken()
        {
            var svc = new JWTService(this.testSecret);
            var token = svc.GenerateToken(new JWTContainerModel {
                Claims = new Claim[] {
                    new Claim(ClaimTypes.Name, "user1"),
                    new Claim(ClaimTypes.Email, "user1@a.com")
                }
            });
            Assert.IsFalse(string.IsNullOrEmpty(token));
        }

        [TestMethod]
        public void TestGetTokenClaims()
        {
            var testClaims = new Claim[] {
                    new Claim(ClaimTypes.Name, "user1"),
                    new Claim(ClaimTypes.Email, "user1@a.com"),
                    new Claim(ClaimTypes.Country, "CN"),
                };
            var svc = new JWTService(this.testSecret);
            var token = svc.GenerateToken(new JWTContainerModel
            {
                Claims = testClaims
            });

            svc = new JWTService(this.testSecret);
            var claimsReturn = svc.GetTokenClaims(token);
            Assert.IsTrue(testClaims.Length <= claimsReturn.Count());
            foreach(var claim in testClaims)
            {
                var match = claimsReturn.Where(x => x.Type == claim.Type && x.Value == claim.Value);
                Assert.AreEqual(1, match.Count());
            }
        }

        [TestMethod]
        public void TestGetTokenClaimsFromInvalidToken()
        {
            var testClaims = new Claim[] {
                    new Claim(ClaimTypes.Name, "user1"),
                    new Claim(ClaimTypes.Email, "user1@a.com"),
                    new Claim(ClaimTypes.Country, "CN"),
                };
            var svc = new JWTService(this.testSecret);
            var token = svc.GenerateToken(new JWTContainerModel
            {
                Claims = testClaims
            });

            svc = new JWTService(this.testSecret2);
            Assert.ThrowsException<SecurityTokenInvalidSignatureException>(() => svc.GetTokenClaims(token));
        }

        [TestMethod]
        public void TestIsTokenValid()
        {
            var testClaims = new Claim[] {
                    new Claim(ClaimTypes.Name, "user1"),
                    new Claim(ClaimTypes.Email, "user1@a.com"),
                    new Claim(ClaimTypes.Country, "CN"),
                };
            var svc = new JWTService(this.testSecret);
            var token = svc.GenerateToken(new JWTContainerModel
            {
                Claims = testClaims
            });

            svc = new JWTService(this.testSecret);
            Assert.IsTrue(svc.IsTokenValid(token));
        }

        [TestMethod]
        public void TestIsTokenInValid()
        {
            var testClaims = new Claim[] {
                    new Claim(ClaimTypes.Name, "user1"),
                    new Claim(ClaimTypes.Email, "user1@a.com"),
                    new Claim(ClaimTypes.Country, "CN"),
                };
            var svc = new JWTService(this.testSecret);
            var token = svc.GenerateToken(new JWTContainerModel
            {
                Claims = testClaims
            });

            svc = new JWTService(this.testSecret2);
            Assert.IsFalse(svc.IsTokenValid(token));
        }
    }
}

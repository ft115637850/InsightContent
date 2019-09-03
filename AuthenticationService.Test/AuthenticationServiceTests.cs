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
        private string testSecret = @"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";

        private string testSecret2 = @"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";
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

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Klootzakken.Web.Controllers
{
    [Authorize]
    public class TokenController : Controller
    {
        public class TokenOptions
        {
            public string Issuer { get; set; }
            public string Audience { get; set; }
            public TimeSpan Expiration { get; set; } = TimeSpan.FromDays(31);
            public SigningCredentials SigningCredentials { get; set; }
        }

        public static TokenOptions Options { get; } = new TokenOptions();

        [HttpGet, HttpPost]
        public IActionResult Index()
        {
            var expiration = Options.Expiration;
            var response = CreateToken(expiration);

            return Ok(response);
        }

        private object CreateToken(TimeSpan expiration)
        {
            var firstIdentity = User.Identity as ClaimsIdentity ?? User.Identities.First();
            var response = CreateTokenFor(expiration, firstIdentity);
            return response;
        }

        internal static object CreateTokenFor(TimeSpan expiration, ClaimsIdentity firstIdentity)
        {
            var now = DateTime.UtcNow;
            var bearerToken = new JwtSecurityTokenHandler().CreateEncodedJwt(
                issuer: Options.Issuer,
                audience: Options.Audience,
                subject: firstIdentity,
                notBefore: now,
                expires: now.Add(expiration),
                issuedAt: now,
                signingCredentials: Options.SigningCredentials
            );
            var response = new
            {
                access_token = bearerToken,
                expires_in = (int) expiration.TotalSeconds
            };
            return response;
        }
    }
}
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
            public TimeSpan Expiration { get; set; } = TimeSpan.FromDays(1);
            public SigningCredentials SigningCredentials { get; set; }
        }

        public static TokenOptions Options { get; } = new TokenOptions();
        [HttpGet,HttpPost]
        public IActionResult Index()
        {
            var now = DateTime.UtcNow;
            var firstIdentity = User.Identity as ClaimsIdentity ?? User.Identities.First();
            var bearerToken = new JwtSecurityTokenHandler().CreateEncodedJwt(
                issuer: Options.Issuer,
                audience: Options.Audience,
                subject: firstIdentity,
                notBefore: now,
                expires: now.Add(Options.Expiration),
                issuedAt: now,
                signingCredentials: Options.SigningCredentials
            );
            var response = new
            {
                access_token = bearerToken,
                expires_in = (int)Options.Expiration.TotalSeconds
            };

            return Ok(response);
        }
    }
}
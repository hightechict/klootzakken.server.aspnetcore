using System.Linq;
using System.Security.Claims;
using Klootzakken.Core.Model;

namespace Klootzakken.Api.Controllers
{
    internal static class HelperExtensions
    {
        public static User AsGameUser(this ClaimsPrincipal claim)
        {
            var userId = claim.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var nameClaim = (claim.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name) ?? claim.Claims.Single(c => c.Type == ClaimTypes.Email));
            var userName = nameClaim.Value;
            return new User(userId, userName);
        }
    }
}
using System.Security.Claims;

namespace api.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            return user.Claims.SingleOrDefault(x => x.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value;
        }
    }
}
using System.Security.Claims;

namespace Collector.AspnetCore.Proxy.OAuth
{
    internal class UserClaimHelpers
    {
        public static string Name(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal?.FindFirst(CustomClaimTypes.Name)?.Value;
        }

        public static string NationalCountry(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal?.FindFirst(CustomClaimTypes.NationalCountry)?.Value;
        }

        public static string NationalId(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal?.FindFirst(CustomClaimTypes.NationalId)?.Value;
        }

        public static LoggedInUser GetLoggedInUser(ClaimsPrincipal claimsPrincipal)
        {
            return new LoggedInUser(NationalCountry(claimsPrincipal), NationalId(claimsPrincipal), Name(claimsPrincipal));
        }
    }
}
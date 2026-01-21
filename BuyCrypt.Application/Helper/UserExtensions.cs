using System.Security.Claims;

namespace BuyCrypt.Application.Helper
{

    public static class UserExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? user.FindFirstValue("sub");

            return Guid.Parse(userId!);
        }
    }

}

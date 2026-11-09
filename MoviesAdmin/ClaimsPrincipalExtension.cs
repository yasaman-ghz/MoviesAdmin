using System;
using System.Security.Claims;

namespace MoviesAdmin
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Extends ClaimsPrincipal to retrieve the unique identifier of the authenticated user.
        /// </summary>
        public static string GetUserId(this ClaimsPrincipal user) =>
            user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User is not authenticated");
    }
}

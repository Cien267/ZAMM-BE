using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Zamm.Application.Payloads.Responses;

namespace Zamm.Application.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                throw new ResponseErrorObject(
                    "User is not authenticated",
                    StatusCodes.Status401Unauthorized
                );
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? user.FindFirst("Id")?.Value
                              ?? user.FindFirst("UserId")?.Value;
        
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new ResponseErrorObject(
                    "Invalid token or user ID not found",
                    StatusCodes.Status401Unauthorized
                );
            }

            return userId;
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value
                   ?? user.FindFirst("UserName")?.Value
                   ?? throw new ResponseErrorObject("Username not found", StatusCodes.Status401Unauthorized);
        }

        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value
                   ?? user.FindFirst("Email")?.Value
                   ?? throw new ResponseErrorObject("Email not found", StatusCodes.Status401Unauthorized);
        }
    }
}


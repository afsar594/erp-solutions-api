using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace Inspire.Erp.Application.User
{
    public class CurrentUserService : ICurrentUserService
    {
        public long UserId { get; }
        public string Username { get; }
        public string Role { get; }
        public string GroupId { get; }
        public string DepartmentId { get; }

        public long LocationId { get; }

        public bool IsAdmin => Role == "Admin";

        public CurrentUserService(IHttpContextAccessor accessor)
        {
            var user = accessor.HttpContext?.User;
            if (user != null)
            {
                UserId = user.HasClaim(c => c.Type == ClaimTypes.NameIdentifier) ? Convert.ToInt64(user.FindFirst(ClaimTypes.NameIdentifier)!.Value) : -1;
                Username = user.Identity?.Name ?? string.Empty;
                Role = user.HasClaim(c => c.Type == ClaimTypes.Role) ? user.FindFirst(ClaimTypes.Role)!.Value : string.Empty;
                GroupId = user.HasClaim(c => c.Type == "GroupId") ? user.FindFirst("GroupId")!.Value : string.Empty;
                DepartmentId = user.HasClaim(c => c.Type == "DepartmentId") ? user.FindFirst("DepartmentId")!.Value : string.Empty;
                LocationId = user.HasClaim(c => c.Type == "LocationId") ? long.Parse(user.FindFirst("LocationId")!.Value) : -1;
            }
        }

    }

}

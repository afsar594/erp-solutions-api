using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.User
{
    public class CurrentUserService : ICurrentUserService
    {
        public long UserId { get; }
        public string Username { get; }
        public string Role { get; }
        public string GroupId { get;}
        public string DepartmentId { get;}

        public CurrentUserService(IHttpContextAccessor accessor)
        {
            var user = accessor.HttpContext?.User;
            if (user != null)
            {
                UserId = Convert.ToInt64(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                Username = user.Identity?.Name;
                Role = user.FindFirst(ClaimTypes.Role)?.Value;
                GroupId = user.FindFirst("GroupId")?.Value;
                DepartmentId = user.FindFirst("DepartmentId")?.Value;
            }
        }
    }

}

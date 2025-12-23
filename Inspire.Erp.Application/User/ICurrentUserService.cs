using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.User
{
    public interface ICurrentUserService
    {
        long UserId { get; }
        string Username { get; }
        string Role { get; }
        string GroupId { get;}
        string DepartmentId { get; }
        long LocationId { get; }
        bool IsAdmin { get; }
    }

}

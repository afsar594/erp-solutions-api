using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.User
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string Username { get; }
        string Role { get; }
        string GroupId { get;}
        string DepartmentId { get;}
    }

}

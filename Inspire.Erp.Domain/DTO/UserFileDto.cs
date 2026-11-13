using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Domain.DTO
{
    public class UserFileDto
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public int GroupId { get; set; }
    }
}

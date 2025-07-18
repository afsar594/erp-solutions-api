using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Domain.Entities
{
    public class DefaultApprover
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Sno { get; set; }
        public bool IsActive { get; set; }
        public string? VoucherType { get; set; }
    }
}

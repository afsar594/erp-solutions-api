using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Entities
{
    public class Menu
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool? IsDeleted { get; set; }
        public int? ParentId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string RouteUrl { get; set; }
        public string IconClass { get; set; }
        // Navigation property for many-to-many relationship
        public ICollection<WorkGroupMenuDetail> MenusWorkgroupDetails { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Inspire.Erp.Domain.Entities
{
    public partial class WorkGroupMaster
    {
       
        public int WorkGroupMasterWorkGroupId { get; set; }
        public string WorkGroupMasterWorkGroupName { get; set; }
        public bool? WorkGroupMasterWorkGroupDelStatus { get; set; }

        public string WorkGroupMasterWorkGroupdashboardType { get; set; }
        //public ICollection<WorkGroupMenuDetail> MenusWorkgroupDetails { get; set; }

        public ICollection<WorkGroupPermissions> WorkGroupPermissions { get; set; }
        public WorkGroupMaster()
        {
            WorkGroupPermissions = new HashSet<WorkGroupPermissions>();

        }
        [JsonIgnore]
        public virtual ICollection<UserFile> UserFiles { get; set; } = new List<UserFile>();
    }
}

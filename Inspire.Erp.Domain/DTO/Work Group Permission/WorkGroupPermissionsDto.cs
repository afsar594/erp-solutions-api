using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.DTO.WorkGropPermission
{
    public class WorkGroupPermissionsDto
    {
        public string WorkGroupPermissionsFormName { get; set; }
        public int? WorkGroupPermissionsFormId { get; set; }
        public int? WorkGroupPermissionsWorkGroupId { get; set; }
        public bool? WorkGroupPermissionsUallow { get; set; }
        public bool? WorkGroupPermissionsUadd { get; set; }
        public bool? WorkGroupPermissionsUedit { get; set; }
        public bool? WorkGroupPermissionsUdelete { get; set; }
        public bool? WorkGroupPermissionsUview { get; set; }
        public bool? WorkGroupPermissionsUprint { get; set; }
    }
}

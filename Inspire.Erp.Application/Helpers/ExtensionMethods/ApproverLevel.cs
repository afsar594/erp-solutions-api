using Inspire.Erp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Helpers.ExtensionMethods
{
    public static class ApproverLevel
    {
        public static EnumApproverLevels GetApproverLevel(this int sNo)
        {
            return sNo switch
            {
                1 => EnumApproverLevels.Level1,
                2 => EnumApproverLevels.Level2,
                3 => EnumApproverLevels.Level3,
                _ => EnumApproverLevels.None,
            };
        }
    }
}

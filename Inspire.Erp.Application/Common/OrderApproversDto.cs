using Inspire.Erp.Domain.Enums;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Common
{
    public class OrderApproversDto
    {
        public OrderApproversDto()
        {
            Emails = new List<ApproverOptEmail>();
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public EnumApproverLevels Level{ get; set; }
        public List<ApproverOptEmail> Emails { get; set; }
    }
    public class ApproverOptEmail
    {
        public bool OptEmail { get; set; }
        public EmailAddress Email { get; set; }
    }
}

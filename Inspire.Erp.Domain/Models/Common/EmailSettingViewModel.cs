using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Models.Common
{
    public class EmailSettingViewModel
    {
        public string FromEmail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Key { get; set; }
        public List<EmailEntry> CCEmails { get; set; }
        public List<EmailEntry> BccEmails { get; set; }
        public string POPRTemplateId { get; set; }
        public string CustomerQuotationTemplateId { get; set; }
        public string TemplateId { get; set; }
        public string MainUrl { get; set; }
        public string AutoLoginUrl { get; set; }
        public string RequestedBy { get; set; }
        public string Department { get; set; }
        public string SupportEmail { get; set; }

    }
    public class EmailEntry
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}

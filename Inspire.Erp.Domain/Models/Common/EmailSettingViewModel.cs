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
        public List<string> CCEmails { get; set; }
        public string POPRTemplateId { get; set; }
        public string TemplateId { get; set; }
        public string MainUrl { get; set; }

    }
}

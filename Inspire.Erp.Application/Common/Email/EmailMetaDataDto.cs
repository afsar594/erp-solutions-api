using Inspire.Erp.Domain.Enums;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inspire.Erp.Application.Common.Email
{
    public class EmailMetaDataDto
    {
       
        public EnumOrderForms FormType { get; set; }
        public long ApproverId { get; set; }
        public EnumApproverLevels ApproverLevel { get; set; }
        public bool OptEmail { get; set; }
        public int NotificationDays { get; set; }
        public decimal MinAmount { get; set; } = 0;
        public decimal MaxAmount { get; set; } = 0;
        public int? JobId { get; set; }
        public string? Department { get; set; }
        public int Sno { get; set; }
        public List<UserEmail> MetaData { get; set; }
    }
    public class UserEmail
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public  List<CCAndBccMailDto> CCEmails { get; set; }
        public  List<CCAndBccMailDto> BccEmails { get; set; }
    }
    public class CCAndBccMailDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
   
}

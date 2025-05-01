using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Inspire.Erp.Domain.Models.Approvals
{
    public class ApprovalResponse
    {
        public int Id { get; set; }
        public string? VoucherType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? Status { get; set; }
        public int? CreatedBy { get; set; }
        public int? LocationId { get; set; }
        public List<ApprovalDetailResponse> ApprovalDetails { get; set; }
    }
    public class ApprovalDetailResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public bool? Status { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int? ApprovalId { get; set; }
    }
    public class Permissionapproval
    {
        public int ApId { get; set; }
        public int ApprovalformId { get; set; }
        public int? UserId { get; set; }
        public int? LevelId { get; set; }

        public string? Username { get; set; }
        public DateTime? CreatedDate { get; set; }

        [NotMapped]
        public Boolean isActive { get; set; }
       
    }



    public class ApproverLevelSettingsDto
    {
        public ApproverLevelSettingsDto()
        {
            ApproverLevels = new List<Approvers>();
        }
        public EnumOrderForms FormType { get; set; }
        public int JobId { get; set; }
        public List<Approvers> ApproverLevels { get; set; }
        
    }

    public class Approvers
    {
        public EnumApproverLevels ApproverLevel { get; set; }
        public long ApproverId { get; set; }
        public bool OptEmail { get; set; }
        public int NotificationDays { get; set; }
        public int MinAmount { get; set; }
        public int MaxAmount { get; set; }
        public List<UserMetaDataDto> MetaData { get; set; }
    }
    public class UserMetaDataDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public List<CCBccMailDto> CCEmails { get; set; }
        public List<CCBccMailDto> BccEmails { get; set; }
    }
    public class CCBccMailDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

}

using Inspire.Erp.Application.Authentication.Inerfaces;
using Inspire.Erp.Application.Common.Email;
using Inspire.Erp.Application.Email.Interfaces;
using Inspire.Erp.Application.Helpers;
using Inspire.Erp.Application.NewFolder.Interfaces;
using Inspire.Erp.Application.User;
using Inspire.Erp.Domain.Constants;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Models.Common;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Email.Implementations
{
    public class OrderEmailNotificationService : IOrderEmailNotificationService
    {
        private readonly PurchaseOrderRequisitionHelper _purchaseOrderRequisitionHelper;
        private readonly IEmailNotificationServices _emailNotificationServices;
        private readonly IRepository<OrderApprovalHierarchy> _orderApprovalHierarchy;
        private readonly IRepository<DepartmentMaster> _departmentMaster;
        private readonly EmailSettingViewModel _mailSettings;
        private readonly IRepository<UserFile> _userFile;
        private readonly ILogger<OrderEmailNotificationService> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDocsAttachmentService _docsAttachmentService;
        private readonly IloginService _loginService;
        public OrderEmailNotificationService(IEmailNotificationServices emailNotificationServices,
            IRepository<OrderApprovalHierarchy> orderApprovalHierarchy,
            IOptions<EmailSettingViewModel> mailSettings,
            ILogger<OrderEmailNotificationService> logger,
            IRepository<UserFile> userFile, IRepository<ApproverLevelSettings> approverLevelSettings, IRepository<DepartmentMaster> departmentMaster, ICurrentUserService currentUserService, IDocsAttachmentService docsAttachmentService, IloginService loginService)
        {
            _userFile = userFile;
            _purchaseOrderRequisitionHelper = new PurchaseOrderRequisitionHelper(userFile, approverLevelSettings);
            _emailNotificationServices = emailNotificationServices;
            _orderApprovalHierarchy = orderApprovalHierarchy;
            _mailSettings = mailSettings.Value;
            _logger = logger;
            _departmentMaster = departmentMaster;
            _currentUserService = currentUserService;
            _docsAttachmentService = docsAttachmentService;
            _loginService = loginService;
        }

        public async Task SendOrderEmailNotificationAsync(
            EnumOrderForms orderType, EnumApproverLevels level, string subTitle,
            string poNumber, DateTime date, string supplier, decimal totalAmount,
            int orderId, int jobId, string jobNo, string jobName, bool IsNewEntery,
            string requestedBy, string settingValue, List<string> attachments,
            int approverId = -1, FileResultDto fileResult = null, long originatorId = default)
        {
            try
            {
                if (settingValue == InspireConstants.ApprovalAsNoApproval)
                    return;

                _logger.LogInformation("Starting email notification for Order ID: {OrderId}, Level: {Level}", orderId, level);

                var approvers = await _purchaseOrderRequisitionHelper.GetOrderApproverNameAndEmail(orderType, level, jobId);
                if (approvers == null || !approvers.Any())
                {
                    _logger.LogWarning("No approvers found for Order ID: {OrderId}, Level: {Level}, Job Name: {JobName}", orderId, level, jobName);
                    return;
                }

                var orderApprovers = approvers
                    .Select(x => new OrderApprovalHierarchy
                    {
                        ApproverId = x.ApproverId,
                        OrderId = orderId,
                        Status = EnumApprovalStatus.Pending,
                        OrderType = orderType,
                        Remarks = "",
                        ApproverLevel = x.ApproverLevel,
                        Sno = x.Sno,
                        FilePath = string.Join(',', attachments)
                    })
                    .ToList();

                if (orderApprovers.Any() && IsNewEntery)
                {
                    _orderApprovalHierarchy.InsertList(orderApprovers);
                    _logger.LogInformation("Approval hierarchy inserted for Order ID: {OrderId}", orderId);
                }

                if (orderType == EnumOrderForms.GeneralPurchaseOrder)
                    return;

                long UserId = approverId > -1 ? approverId : Convert.ToInt64(_currentUserService.UserId);
                var Username = await getOriginatorName(originatorId);

                List<UserEmail> toMails;

                if (approverId == -1)
                {
                    var topApprover = approvers
                        .Where(x => x.ApproverLevel == EnumApproverLevels.Level1 && x.OptEmail)
                        .FirstOrDefault();

                    if (topApprover == null)
                        return;

                    toMails = topApprover.MetaData;
                }
                else
                {
                    toMails = approvers
                        .Where(x => x.ApproverLevel == level && x.OptEmail && x.ApproverId == approverId)
                        .OrderBy(x => x.Sno)
                        .SelectMany(x => x.MetaData)
                        .Distinct()
                        .ToList();
                }

                if (toMails.Any())
                {
                    var allApproverLevels = approvers.Select(x => x.ApproverLevel).Distinct().ToList();
                    var isLastLevel = level == allApproverLevels.Max();
                    var actionButtons = new List<object>();
                    var deptName = GetDepartment(UserId);
                    var mergedFileResult = new FileResultDto();

                    if (isLastLevel)
                    {
                        string token = _loginService.GenerateAutoLoginToken(UserId);
                        
                        string approvalRedirectUrl = $"{_mailSettings.MainUrl}purchase-order/approve-email-template?approverLevel={level}&orderId={orderId}&orderType={orderType}&jobId={jobId}&approvalStatus=2&userId={UserId}";
                        string rejectRedirectUrl = $"{_mailSettings.MainUrl}purchase-order/approve-email-template?approverLevel={level}&orderId={orderId}&orderType={orderType}&jobId={jobId}&approvalStatus=3&userId={UserId}";
                        
                        actionButtons.Add(new { ApproveUrl = $"{_mailSettings.AutoLoginUrl}api/auto-login?token={token}&redirect={Uri.EscapeDataString(approvalRedirectUrl)}" });
                        actionButtons.Add(new { RejectUrl = $"{_mailSettings.AutoLoginUrl}api/auto-login?token={token}&redirect={Uri.EscapeDataString(rejectRedirectUrl)}" });
                        
                        mergedFileResult = await _docsAttachmentService.DownloadAllFilesAsOnePdf(orderId, GetOrderTypeKey(orderType)); // Renamed variable to avoid conflict
                    }
                    else
                    {
                        var approvalUrl = $"{_mailSettings.MainUrl}procurement/po-approval?menu=184&vNo={orderId}&type={orderType}".Trim();
                        actionButtons.Add(new { ApproveUrl = $"{_mailSettings.MainUrl}login?returnUrl={Uri.EscapeDataString(approvalUrl)}" });
                    }
                    var rptData = toMails.Select(x => new
                    {
                        SubTitle = subTitle,
                        ApproverName = x.Name,
                        PONumber = poNumber,
                        Date = date.ToString("MMM dd yyyy"),
                        JobName = $"{jobName}/{jobNo}",
                        Supplier = supplier,
                        TotalAmount = totalAmount,
                        RequestedBy = Username,
                        Department = deptName,
                        ActionButtons = actionButtons,
                        SupportEmail = _mailSettings.SupportEmail
                    }).Cast<object>().ToList();

                    string templateId = GetTemplateId(orderType);
                    await _emailNotificationServices.SendTemplatedEmailAsync(toMails, templateId, rptData, attachments, mergedFileResult);
                    _logger.LogInformation("Email sent to approvers: {Emails}", string.Join(", ", toMails.Select(x => x.Email)));
                }

                _logger.LogInformation("Email notification process completed for Order ID: {OrderId}", orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email notification for Order ID: {OrderId}, Level: {Level}", orderId, level);
                throw;
            }
        }

        private async Task<string> getOriginatorName(long originatorId)
        {
            return await _userFile.GetAsQueryable()
                  .Where(x => x.UserId == originatorId)
                  .Select(x => x.UserName)
                  .FirstOrDefaultAsync();
        }

        private string GetOrderTypeKey(EnumOrderForms orderType)
        {
            return orderType switch
            {
                EnumOrderForms.PurchaseOrder => "PO",
                EnumOrderForms.PurchaseRequisition => "PR",
                EnumOrderForms.CustomerQuotation => "CQ",
                EnumOrderForms.GeneralPurchaseOrder => "GPO",
                _ => throw new ArgumentOutOfRangeException(nameof(orderType), $"No key configured for order type: {orderType}")
            };
        }

        private string GetDepartment(long dptId)
        {
            try
            {

                var dptName = (from d in _departmentMaster.GetAsQueryable()
                               join u in _userFile.GetAsQueryable() on d.DepartmentMasterDepartmentId equals u.DepartmentId
                               where u.UserId == dptId || d.DepartmentMasterDepartmentId == dptId
                               select d.DepartmentMasterDepartmentName).FirstOrDefault();

                return dptName ?? string.Empty;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task SendQuotationEmailNotificationAsync(EnumOrderForms orderType, EnumApproverLevels level, QuotationEmailModel emailModel, bool IsNewEntry = true)
        {
            try
            {
                _logger.LogInformation("Starting email notification for Order ID: {OrderId}, Level: {Level}", emailModel.OrderId, level);

                var approvers = await _purchaseOrderRequisitionHelper.GetOrderApproverNameAndEmail(orderType, level, emailModel.JobId);
                if (approvers == null || !approvers.Any())
                {
                    _logger.LogWarning("No approvers found for Order ID: {OrderId}, Level: {Level}", emailModel.OrderId, level);
                    return;
                }
                var toMails = approvers.Where(x => x.ApproverLevel == level && x.OptEmail)
                                 .SelectMany(x => x.MetaData)
                                 .Distinct()
                                 .ToList();
                if (toMails.Any())
                {
                    var rptData = approvers
                        .Where(x => x.ApproverLevel == level)
                        .Select(x => new
                        {
                            company_name = emailModel.CompanyName,
                            customer_name = emailModel.CustomerName,
                            sender_name = emailModel.SenderName,
                            sender_position = emailModel.SenderPosition,
                            email = emailModel.Email,
                            phone = emailModel.Phone,
                            year = emailModel.Year,
                            grand_total = emailModel.GrandTotal,
                            items = emailModel.Items.Select(i => new
                            {
                                name = i.Name,
                                description = i.Description,
                                quantity = i.Quantity,
                                unit_price = i.Unit_Price,
                                total = i.Total
                            })
                        })
                        .Cast<object>()
                        .ToList();

                    var filePaths = new List<string>();

                    string templateId = GetTemplateId(orderType);
                    await _emailNotificationServices.SendTemplatedEmailAsync(toMails, templateId, rptData, filePaths);
                    _logger.LogInformation("Email sent to approvers: {Emails}", string.Join(", ", toMails));
                }

                var orderApprovers = approvers
                    .Select(x => new OrderApprovalHierarchy
                    {
                        ApproverId = x.ApproverId,
                        OrderId = emailModel.OrderId,
                        Status = EnumApprovalStatus.Pending,
                        OrderType = orderType
                    })
                    .ToList();

                if (orderApprovers.Any() && IsNewEntry)
                {
                    _orderApprovalHierarchy.InsertList(orderApprovers);
                    _logger.LogInformation("Approval hierarchy inserted for Order ID: {OrderId}", emailModel.OrderId);
                }

                _logger.LogInformation("Email notification process completed for Order ID: {OrderId}", emailModel.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email notification for Order ID: {OrderId}, Level: {Level}", emailModel.OrderId, level);
                throw;
            }
        }

        public async Task<bool> SendTextEmailAsync(List<UserEmail> toEmails, string subject, string message, List<string> filePaths)
        {
            try
            {

                return await _emailNotificationServices.SendTextEmailAsync(toEmails, subject, message, filePaths);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string GetTemplateId(EnumOrderForms orderType)
        {
            if (new[] { EnumOrderForms.PurchaseOrder, EnumOrderForms.PurchaseRequisition }.Contains(orderType))
                return _mailSettings.POPRTemplateId;

            return orderType switch
            {
                EnumOrderForms.CustomerQuotation => _mailSettings.CustomerQuotationTemplateId,
                _ => throw new ArgumentOutOfRangeException(nameof(orderType), $"No template ID configured for order type: {orderType}")
            };
        }
    }
}

using Inspire.Erp.Application.Authentication.Inerfaces;
using Inspire.Erp.Application.Common.Dtos;
using Inspire.Erp.Application.Common.Email;
using Inspire.Erp.Application.CusNotification;
using Inspire.Erp.Application.Email.Interfaces;
using Inspire.Erp.Application.Helpers;
using Inspire.Erp.Application.NewFolder.Interfaces;
using Inspire.Erp.Application.Settings.Interfaces;
using Inspire.Erp.Application.User;
using Inspire.Erp.Domain.Constants;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Domain.Models.Approvals;
using Inspire.Erp.Domain.Models.Common;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.NotificationBus;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    
        private readonly INotificaionService _notificaionService;

        private IRepository<PurchaseRequisition> _purchaseRequisitionRepository;
        private IRepository<PurchaseOrder> _purchaseOrderRepository;
        private IRepository<JobMaster> _jobrepository;
        private readonly IGeneralSettings _generalSettings;

        public OrderEmailNotificationService(IEmailNotificationServices emailNotificationServices,
            IRepository<OrderApprovalHierarchy> orderApprovalHierarchy,
            IOptions<EmailSettingViewModel> mailSettings,
            ILogger<OrderEmailNotificationService> logger,
            IRepository<UserFile> userFile, IRepository<ApproverLevelSettings> approverLevelSettings, IRepository<DepartmentMaster> departmentMaster, ICurrentUserService currentUserService, IDocsAttachmentService docsAttachmentService, IloginService loginService, INotificaionService notificaionService, IRepository<PurchaseRequisition> purchaseRequisitionRepository, IRepository<JobMaster> jobrepository, IGeneralSettings generalSettings, IRepository<PurchaseOrder> purchaseOrderRepository)
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
            _notificaionService = notificaionService;
            _purchaseRequisitionRepository = purchaseRequisitionRepository;
            _jobrepository = jobrepository;
            _generalSettings = generalSettings;
            _purchaseOrderRepository = purchaseOrderRepository;
        }



        public async Task SendOrderEmailNotificationAsync(EnumOrderForms orderType, EnumApproverLevels level, string subTitle, string poNumber,
                DateTime date, string supplier, decimal totalAmount, int orderId, int jobId,
                string jobNo, string jobName, bool IsNewEntery, string requestedBy, string settingValue,
                List<string> attachments, int approverId = -1, FileResultDto fileResult = null, long originatorId = default)
        {
            try
            {
                if (settingValue == InspireConstants.ApprovalAsNoApproval)
                    return;

                _logger.LogInformation("Starting email notification for OrderId: {OrderId}, Level: {Level}", orderId, level);

                var approvers = await _purchaseOrderRequisitionHelper
                    .GetOrderApproverNameAndEmail(orderType, level, jobId);

                if (approvers == null || !approvers.Any())
                {
                    _logger.LogWarning("No approvers found. OrderId: {OrderId}, JobId: {JobId}", orderId, jobId);
                    return;
                }

                // 🔹 Build approval hierarchy list
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

                // 🔹 Insert hierarchy + auto approve based on current level
                if (orderApprovers.Any() && IsNewEntery)
                {
                    var currentApproverLevel = approvers.Any(x => x.ApproverId == _currentUserService.UserId)
                                                ? approvers.FirstOrDefault(x => x.ApproverId == _currentUserService.UserId).ApproverLevel
                                                : EnumApproverLevels.None;
                    ApplyAutoApprovalByLevel(orderApprovers, currentApproverLevel);
                    _orderApprovalHierarchy.InsertList(orderApprovers);

                    _logger.LogInformation(
                        "Approval hierarchy inserted for OrderId: {OrderId}",
                        orderId);
                }

                // ❌ GPO does not require email
                if (orderType == EnumOrderForms.GeneralPurchaseOrder)
                    return;

                // 🔹 Find NEXT pending approval level
                var nextPendingLevel = orderApprovers
                    .Where(x => x.Status == EnumApprovalStatus.Pending)
                    .OrderBy(x => x.Sno)
                    .Select(x => new { level = x.ApproverLevel, Status = x.Status })
                    .FirstOrDefault();

                // ❌ No email if all approved OR next level is Level3
                if (nextPendingLevel == default || (nextPendingLevel.Status == EnumApprovalStatus.Approved && nextPendingLevel.level == EnumApproverLevels.Level3))
                {
                    _logger.LogInformation("No email sent. Either fully approved or next level is Level3. OrderId: {OrderId}", orderId);
                    return;
                }

                // 🔹 Get recipients ONLY for next pending level
                var toMails = approvers
                    .Where(x =>
                        x.ApproverLevel == nextPendingLevel.level &&
                        x.OptEmail)
                    .OrderBy(x => x.Sno)
                    .SelectMany(x => x.MetaData)
                    .Distinct()
                    .ToList();

                if (!toMails.Any())
                {
                    _logger.LogWarning(
                        "No email recipients found for next level {Level}, OrderId: {OrderId}",
                        nextPendingLevel, orderId);
                    return;
                }

                long userId = approverId > -1
                    ? approverId
                    : Convert.ToInt64(_currentUserService.UserId);

                var userName = await getOriginatorName(originatorId);
                var deptName = GetDepartment(userId);

                // 🔹 Action buttons (approval screen only)
                var actionButtons = new List<object>();
                if (level == EnumApproverLevels.Level3)
                {
                    string token = _loginService.GenerateAutoLoginToken(userId);
                    string approvalRedirectUrl = $"{_mailSettings.MainUrl}purchase-order/approve-email-template?approverLevel={level}&orderId={orderId}&orderType={orderType}&jobId={jobId}&approvalStatus=2&userId={userId}";
                    string rejectRedirectUrl = $"{_mailSettings.MainUrl}purchase-order/approve-email-template?approverLevel={level}&orderId={orderId}&orderType={orderType}&jobId={jobId}&approvalStatus=3&userId={userId}";
                    actionButtons.Add(new
                    {
                        ApproveUrl = $"{_mailSettings.AutoLoginUrl}api/auto-login?token={token}&redirect={Uri.EscapeDataString(approvalRedirectUrl)}",
                        RejectUrl = $"{_mailSettings.AutoLoginUrl}api/auto-login?token={token}&redirect={Uri.EscapeDataString(rejectRedirectUrl)}"
                    });

                    fileResult = await _docsAttachmentService.DownloadAllFilesAsOnePdf(orderId, GetOrderTypeKey(orderType));
                }
                else
                {
                    string approvalUrl = $"{_mailSettings.MainUrl}procurement/po-approval?menu=184&vNo={orderId}&type={orderType}".Trim();

                    actionButtons.Add(new
                    {
                        ApproveUrl = $"{_mailSettings.MainUrl}login?returnUrl={Uri.EscapeDataString(approvalUrl)}"
                    });
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
                    RequestedBy = userName,
                    Department = deptName,
                    ActionButtons = actionButtons,
                    SupportEmail = _mailSettings.SupportEmail
                }).Cast<object>().ToList();

                string templateId = GetTemplateId(orderType);

                await _emailNotificationServices.SendTemplatedEmailAsync(
                    toMails,
                    templateId,
                    rptData,
                    attachments,
                    fileResult);

                _logger.LogInformation(
                    "Email sent to next approval level {Level} for OrderId: {OrderId}",
                    nextPendingLevel, orderId);

                await notifyConcernUser(
                    nextPendingLevel.level,
                    userId,
                    orderType,
                    poNumber,
                    userName,
                    orderId);

                _logger.LogInformation(
                    "Email notification process completed for OrderId: {OrderId}",
                    orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send email notification for OrderId: {OrderId}, Level: {Level}",
                    orderId, level);
                throw;
            }
        }


        private async Task notifyConcernUser(EnumApproverLevels level, long userId, EnumOrderForms orderType, string poNumber, string username, int orderId)
        {
            try
            {
                var notificationToAdd = new NotificationDto()
                {
                    Message = $"{orderType} '{poNumber}' pending approval.",
                    IsRead = false,
                    NotificationId = 0,
                    RouteUrl = $"procurement/po-approval?menu=184&vNo={orderId}&type={orderType}".Trim(),
                    UserId = userId
                };
                await _notificaionService.CreateNotificationAsync(notificationToAdd);
                _logger.LogInformation("notification added {notificationToAdd}", notificationToAdd);
            }
            catch (Exception ex)
            {
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

        public void ApplyAutoApprovalByLevel(List<OrderApprovalHierarchy> orderApprovers, EnumApproverLevels currentUserLevel)
        {
            if (orderApprovers == null || !orderApprovers.Any())
                return;

            var userId = _currentUserService.UserId;
            var now = DateTime.UtcNow;

            var cSno = orderApprovers.FirstOrDefault(x => x.ApproverId == userId).Sno;
            var approversToApprove = orderApprovers
                .Where(x => (int)x.ApproverLevel <= (int)currentUserLevel);

            foreach (var approver in approversToApprove)
            {
                if (cSno != null && approver.Sno > cSno && approver.ApproverLevel >= currentUserLevel) continue;

                // Optional safety: don’t overwrite if already approved
                if (approver.Status == EnumApprovalStatus.Approved)
                    continue;

                approver.Status = EnumApprovalStatus.Approved;
                approver.ApprovedBy = (int)userId;
                approver.ApprovedDate = now;
                approver.Remarks = $"Auto Approved by {_currentUserService.Username}";
            }
        }



        private bool IsApprover(long id) => _currentUserService.UserId == id;



        public async Task<ApiResponse<bool>> ResendEmailAsync(EnumOrderForms orderType, EnumApproverLevels level, long orderId, int approverId , CancellationToken cancellationToken = default)
        {
            try
            {

                return orderType switch
                {
                    EnumOrderForms.PurchaseOrder => await ResendPurchaseOrderEmail(orderId, level, approverId),
                    EnumOrderForms.PurchaseRequisition => await ResendPurchaseRequisition(orderId, level, approverId),
                    _ => ApiResponse<bool>.Fail("No order type were found")
                };

            }
            catch (Exception)
            {

                throw;
            }
        }


        private async Task<ApiResponse<bool>> ResendPurchaseRequisition(long orderId, EnumApproverLevels level, int approverId)
        {
            try
            {
                var purchaseRequisition = await _purchaseRequisitionRepository.GetAsQueryable().AsNoTracking()
                                          .FirstOrDefaultAsync(p => p.PurchaseRequisitionId == orderId);
                var Username = _currentUserService.Username;
                var jobId = purchaseRequisition?.PurchaseRequisitionJobId ?? -1;
                var jobDetail = await _jobrepository.GetAsQueryable().AsNoTracking().FirstOrDefaultAsync(x => x.JobMasterJobId == jobId);
                var settingValue = await _generalSettings.GetApproverStatusAsync(purchaseRequisition.PurchaseRequisitionJobId.ToString(), nameof(EnumOrderForms.PurchaseRequisition));

                await SendOrderEmailNotificationAsync(EnumOrderForms.PurchaseRequisition,
                                                      level,
                                                      "Requisition",
                                                      purchaseRequisition.PurchaseRequisitionNo,
                                                      purchaseRequisition.PurchaseRequisitionDate,
                                                      purchaseRequisition.PurchaseRequisitionPartyName,
                                                      (decimal)purchaseRequisition.PurchaseRequisitionNetAmount,
                                                      (int)purchaseRequisition.PurchaseRequisitionId,
                                                      (int)jobId,
                                                      jobDetail == null ? "" : jobDetail.JobMasterJobNumber,
                                                      jobDetail == null ? "" : jobDetail.JobMasterJobName,
                                                      false,
                                                      Username,
                                                       settingValue.GeneralSettingValue,
                                                       new List<string> { string.Empty },
                                                       approverId,
                                                       null,
                                                       (long)purchaseRequisition.PurchaseRequisitionUserId
                                                       );
                return ApiResponse<bool>.Success(true, "Email resent successfully");
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<ApiResponse<bool>> ResendPurchaseOrderEmail(long orderId, EnumApproverLevels level, int approverId )
        {
            try
            {
                var purchaseOrder = await _purchaseOrderRepository.GetAsQueryable().AsNoTracking()
                                            .FirstOrDefaultAsync(p => p.PurchaseOrderId == orderId);
                var jobId = purchaseOrder?.PurchaseOrderJobId ?? -1;
                var jobDetail = await _jobrepository.GetAsQueryable().AsNoTracking().FirstOrDefaultAsync(x => x.JobMasterJobId == jobId);
                var settingValue = await _generalSettings.GetApproverStatusAsync(purchaseOrder.PurchaseOrderJobId.ToString(), nameof(EnumOrderForms.PurchaseOrder));

                await SendOrderEmailNotificationAsync(EnumOrderForms.PurchaseOrder,
                                                      level,
                                                      "Order",
                                                      purchaseOrder.PurchaseOrderNo,
                                                      purchaseOrder.PurchaseOrderDate,
                                                      purchaseOrder.PurchaseOrderPartyName,
                                                      (decimal)purchaseOrder.PurchaseOrderNetAmount,
                                                      (int)purchaseOrder.PurchaseOrderId,
                                                      (int)jobId,
                                                      jobDetail == null ? "" : jobDetail.JobMasterJobNumber,
                                                      jobDetail == null ? "" : jobDetail.JobMasterJobName,
                                                      false,
                                                      _currentUserService.Username ?? "",
                                                      settingValue.GeneralSettingValue,
                                                      new List<string>() { string.Empty }, approverId,
                                                      null,
                                                      (long)purchaseOrder.PurchaseOrderUserId);

                return ApiResponse<bool>.Success(true, "Email resent successfully");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

using Inspire.Erp.Application.Common.Email;
using Inspire.Erp.Application.Email.Interfaces;
using Inspire.Erp.Application.Helpers;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Models.Approvals;
using Inspire.Erp.Domain.Models.Common;
using Inspire.Erp.Infrastructure.Database.Repositoy;
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
        private readonly EmailSettingViewModel _mailSettings;
        private readonly ILogger<OrderEmailNotificationService> _logger;

        public OrderEmailNotificationService(IEmailNotificationServices emailNotificationServices,
            IRepository<OrderApprovalHierarchy> orderApprovalHierarchy,
            IOptions<EmailSettingViewModel> mailSettings,
            ILogger<OrderEmailNotificationService> logger,
            IRepository<UserFile> userFile, IRepository<ApproverLevelSettings> approverLevelSettings)
        {
            _purchaseOrderRequisitionHelper = new PurchaseOrderRequisitionHelper(userFile, approverLevelSettings); ;
            _emailNotificationServices = emailNotificationServices;
            _orderApprovalHierarchy = orderApprovalHierarchy;
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task SendOrderEmailNotificationAsync(
            EnumOrderForms orderType,
            EnumApproverLevels level,
            string subTitle,
            string poNumber,
            DateTime date,
            string supplier,
            decimal totalAmount,
            int orderId, string jobNo, string jobName, bool IsNewEntery = true)
        {
            try
            {
                _logger.LogInformation("Starting email notification for Order ID: {OrderId}, Level: {Level}", orderId, level);

                var approvers = await _purchaseOrderRequisitionHelper.GetOrderApproverNameAndEmail(orderType, level);
                if (approvers == null || !approvers.Any())
                {
                    _logger.LogWarning("No approvers found for Order ID: {OrderId}, Level: {Level}", orderId, level);
                    return;
                }
                var orderApprovers = approvers
                  .Select(x => new OrderApprovalHierarchy
                  {
                      ApproverId = x.Id,
                      OrderId = orderId,
                      Status = EnumApprovalStatus.Pending,
                      OrderType = orderType,
                      Remarks = ""
                  })
                  .ToList();

                if (orderApprovers.Any() && IsNewEntery)
                {
                    _orderApprovalHierarchy.InsertList(orderApprovers);
                    _logger.LogInformation("Approval hierarchy inserted for Order ID: {OrderId}", orderId);
                }

                var approvalUrl = $"{_mailSettings.MainUrl}procurement/po-approval?menu=184&vNo={orderId}&type={orderType}".Trim();
                var loginUrl = $"{_mailSettings.MainUrl}login?returnUrl={Uri.EscapeDataString(approvalUrl)}";

                var toMails = approvers.Where(x => x.Level == level)
                                 .SelectMany(x => x.Emails.Where(e => e.OptEmail).Select(e => e.Email))
                                 .Distinct()
                                 .ToList();
                if (toMails.Any())
                {
                    var rptData = approvers
                        .Where(x => x.Level == level)
                        .Select(x => new
                        {
                            SubTitle = subTitle,
                            ApproverName = x.Name,
                            PONumber = poNumber,
                            Date = date.ToString("MMM dd yyyy"),
                            JobName = $"{jobName}/{jobNo}",
                            Supplier = supplier,
                            TotalAmount = totalAmount,
                            RequestedBy = _mailSettings.RequestedBy,
                            Department = _mailSettings.Department,
                            ApprovalLink = loginUrl,
                            SupportEmail = _mailSettings.SupportEmail
                        })
                        .Cast<object>()
                        .ToList();

                    var filePaths = new List<string>();

                    string templateId = GetTemplateId(orderType);
                    await _emailNotificationServices.SendTemplatedEmailAsync(toMails, templateId, rptData, filePaths);
                    _logger.LogInformation("Email sent to approvers: {Emails}", string.Join(", ", toMails));
                }



                _logger.LogInformation("Email notification process completed for Order ID: {OrderId}", orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email notification for Order ID: {OrderId}, Level: {Level}", orderId, level);
                throw;
            }
        }

        public async Task SendOrderEmailNotificationAsync<T>(EnumOrderForms orderType, EnumApproverLevels level, string subTitle, string poNumber, DateTime date, string supplier, decimal totalAmount, int orderId, List<T> items, bool IsNewEntry = true)
        {
            try
            {
                _logger.LogInformation("Starting email notification for Order ID: {OrderId}, Level: {Level}", orderId, level);

                var approvers = await _purchaseOrderRequisitionHelper.GetOrderApproverNameAndEmail(orderType, level);
                if (approvers == null || !approvers.Any())
                {
                    _logger.LogWarning("No approvers found for Order ID: {OrderId}, Level: {Level}", orderId, level);
                    return;
                }
                var toMails = approvers
                                 .SelectMany(x => x.Emails.Where(e => e.OptEmail).Select(e => e.Email))
                                 .Distinct()
                                 .ToList();
                if (toMails.Any())
                {
                    var rptData = approvers
                        .Where(x => x.Level == level)
                        .Select(x => new
                        {
                            SubTitle = subTitle,
                            ApproverName = x.Name,
                            PONumber = poNumber,
                            Date = date.ToString("MMM dd yyyy"),
                            Supplier = supplier,
                            TotalAmount = totalAmount,
                            RequestedBy = _mailSettings.RequestedBy,
                            Department = _mailSettings.Department,
                            ApprovalLink = $"{_mailSettings.MainUrl}{orderId}".Trim(),
                            SupportEmail = _mailSettings.SupportEmail
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
                        ApproverId = x.Id,
                        OrderId = orderId,
                        Status = EnumApprovalStatus.Pending,
                        OrderType = orderType
                    })
                    .ToList();

                if (orderApprovers.Any() && IsNewEntry)
                {
                    _orderApprovalHierarchy.InsertList(orderApprovers);
                    _logger.LogInformation("Approval hierarchy inserted for Order ID: {OrderId}", orderId);
                }

                _logger.LogInformation("Email notification process completed for Order ID: {OrderId}", orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email notification for Order ID: {OrderId}, Level: {Level}", orderId, level);
                throw;
            }
        }

        public async Task SendQuotationEmailNotificationAsync(EnumOrderForms orderType, EnumApproverLevels level, QuotationEmailModel emailModel, bool IsNewEntry = true)
        {
            try
            {
                _logger.LogInformation("Starting email notification for Order ID: {OrderId}, Level: {Level}", emailModel.OrderId, level);

                var approvers = await _purchaseOrderRequisitionHelper.GetOrderApproverNameAndEmail(orderType, level);
                if (approvers == null || !approvers.Any())
                {
                    _logger.LogWarning("No approvers found for Order ID: {OrderId}, Level: {Level}", emailModel.OrderId, level);
                    return;
                }
                var toMails = approvers
                                 .SelectMany(x => x.Emails.Where(e => e.OptEmail).Select(e => e.Email))
                                 .Distinct()
                                 .ToList();
                if (toMails.Any())
                {
                    var rptData = approvers
                        .Where(x => x.Level == level)
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
                        ApproverId = x.Id,
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

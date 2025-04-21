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
            int orderId, bool IsNewEntery = true)
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

                if (orderApprovers.Any() && IsNewEntery)
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

        private string GetTemplateId(EnumOrderForms orderType)
        {
            if (new[] { EnumOrderForms.PurchaseOrder, EnumOrderForms.PurchaseRequisition }.Contains(orderType))
                return _mailSettings.POPRTemplateId;

            return orderType switch
            {
                EnumOrderForms.PurchaseRequisition => _mailSettings.POPRTemplateId,
                EnumOrderForms.CustomerQuotation=> _mailSettings.CustomerQuotationTemplateId,
                _ => throw new ArgumentOutOfRangeException(nameof(orderType), $"No template ID configured for order type: {orderType}")
            };
        }
    }
}

using Inspire.Erp.Application.Email.Interfaces;
using Inspire.Erp.Domain.Models.Common;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Email.Implementations
{
    public class EmailNotificationServices : IEmailNotificationServices
    {
        private readonly ILogger<EmailNotificationServices> _logger;
        private readonly EmailSettingViewModel _mailSettings;

        public EmailNotificationServices(IOptions<EmailSettingViewModel> mailSettings, ILogger<EmailNotificationServices> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }
        public async Task<bool> SendEmailWithCCsAndBccsAsync(List<EmailAddress> toEmails, string templateId, List<object> dynamicTemplateData, List<string> filePaths)
        {
            try
            {
                var client = new SendGridClient(_mailSettings.Key);
                var from = new EmailAddress(_mailSettings.FromEmail, _mailSettings.DisplayName);

                var msg = new SendGridMessage
                {
                    From = from,
                    TemplateId = templateId,
                };

                var personalizations = new List<Personalization>();

                for (int i = 0; i < toEmails.Count; i++)
                {
                    var personalization = new Personalization();
                    personalization.Tos = new List<EmailAddress> { toEmails[i] };

                    // Optional: Attach dynamic template data if provided
                    if (dynamicTemplateData != null && dynamicTemplateData.Count > i)
                    {
                        personalization.TemplateData = dynamicTemplateData[i];
                    }

                    // Add CCs
                    if (_mailSettings.CCEmails != null && _mailSettings.CCEmails.Any())
                    {
                        personalization.Ccs = _mailSettings.CCEmails
                            .Select(x => new EmailAddress(x.Email, x.Name))
                            .ToList();
                    }

                    // Add BCCs
                    if (_mailSettings.BccEmails != null && _mailSettings.BccEmails.Any())
                    {
                        personalization.Bccs = _mailSettings.BccEmails
                            .Select(x => new EmailAddress(x.Email, x.Name))
                            .ToList();
                    }

                    personalizations.Add(personalization);
                }

                msg.Personalizations = personalizations;

                if (filePaths != null && filePaths.Count > 0)
                {
                    msg = GetAttachments(msg, filePaths);
                }

                var response = await client.SendEmailAsync(msg);
                _logger.LogInformation($"Email sent successfully to {string.Join(", ", toEmails.Select(t => t.Email))} with status code {response.StatusCode}");
                return response.StatusCode == System.Net.HttpStatusCode.Accepted; // Should check for 202 Accepted
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> SendEmailAsync(List<EmailAddress> toEmails, string templateId, List<object> dynamicTemplateData, List<string> filePaths)
        {
            try
            {
                return await SendEmailWithCCsAndBccsAsync(toEmails,templateId, dynamicTemplateData, filePaths);
                var client = new SendGridClient(_mailSettings.Key);
                var from = new EmailAddress(_mailSettings.FromEmail, _mailSettings.DisplayName);
                var msg = MailHelper.CreateMultipleTemplateEmailsToMultipleRecipients(from, toEmails, templateId, dynamicTemplateData);
                //var msg = MailHelper.CreateSingleTemplateEmailToMultipleRecipients(from, toEmails, templateId, dynamicTemplateData);
                if (filePaths != null && filePaths.Count > 0)
                    msg = GetAttachments(msg, filePaths);

                msg.AddCcs(_mailSettings.CCEmails.Select(x => new EmailAddress(x.Email,x.Name)).ToList());
                msg.AddBccs(_mailSettings.BccEmails.Select(x => new EmailAddress(x.Email,x.Name)).ToList());

                var response = await client.SendEmailAsync(msg);
                _logger.LogInformation($"Email sent successfully to {string.Join(", ", toEmails)} with status code {response.StatusCode}");
                return response.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
                return false;
            }
        }

        private SendGridMessage GetAttachments(SendGridMessage msg, List<string> filePaths)
        {
            filePaths.ForEach(filePath =>
            {
                msg.AddAttachment(
                    new Attachment
                    {
                        Content = Convert.ToBase64String(File.ReadAllBytes(filePath)),
                        Filename = Path.GetFileName(filePath),
                        Type = GetMimeType(filePath),// "application/pdf",
                        Disposition = "attachment"
                    });
            });

            return msg;
        }

        public static string GetMimeType(string filePath)
        {
            var provider = new FileExtensionContentTypeProvider();
            string mimeType;

            if (!provider.TryGetContentType(filePath, out mimeType))
                mimeType = "application/octet-stream";

            return mimeType;
        }

        public async Task<bool> SendTemplatedEmailAsync(List<EmailAddress> toEmails, string templateId, List<object> dynamicTemplateData, List<string> filePaths)
        {
            return await SendEmailAsync(toEmails, templateId, dynamicTemplateData, filePaths);
        }

        public async Task<bool> SendTemplatedEmailAsync(List<string> toEmails, string templateId, List<object> dynamicTemplateData, List<string> filePaths)
        {
            var emails = new List<EmailAddress>();
            toEmails.ForEach(email => emails.Add(new EmailAddress(email, "Test")));
            return await SendEmailAsync(emails, templateId, dynamicTemplateData, filePaths);
        }
        public async Task<bool> SendTextEmailAsync(List<EmailAddress> toEmails, string subject, string message, List<string> filePaths)
        {
            try
            {
                var client = new SendGridClient(_mailSettings.Key);
                var from = new EmailAddress(_mailSettings.FromEmail, _mailSettings.DisplayName);
                var plainTextContent = "";
                var htmlContent = message;
                var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, toEmails, subject, plainTextContent, htmlContent);
                if (filePaths != null && filePaths.Count > 0)
                    msg = GetAttachments(msg, filePaths);

                var response = await client.SendEmailAsync(msg);
                _logger.LogInformation("Message sent successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return false;
            }
        }


    }
}

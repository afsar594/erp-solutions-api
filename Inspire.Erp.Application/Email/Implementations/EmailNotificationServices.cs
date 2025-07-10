using Inspire.Erp.Application.Common.Email;
using Inspire.Erp.Application.Email.Interfaces;
using Inspire.Erp.Domain.Entities;
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

        public async Task<bool> SendEmailAsync(List<UserEmail> toEmails, string templateId, List<object> dynamicTemplateData, List<string> filePaths, FileResultDto fileResult = null)
        {
            try
            {
                var toMails = toEmails.Select(email => new EmailAddress(email.Email, email.Name)).ToList();
                var client = new SendGridClient(_mailSettings.Key);
                var from = new EmailAddress(_mailSettings.FromEmail, _mailSettings.DisplayName);
                var msg = MailHelper.CreateMultipleTemplateEmailsToMultipleRecipients(from, toMails, templateId, dynamicTemplateData);

                //var msg = MailHelper.CreateSingleTemplateEmailToMultipleRecipients(from, toEmails, templateId, dynamicTemplateData);
                if ((filePaths != null && filePaths.Count > 0) || fileResult is not null)
                    msg = GetAttachments(msg, filePaths, fileResult);

                if (toEmails.All(x => x.CCEmails.Any()))
                    msg.AddCcs(toEmails.SelectMany(x => x.CCEmails.Select(s => new EmailAddress(s.Email, s.Name))).ToList());
                if (toEmails.All(x => x.BccEmails.Any()))
                    msg.AddBccs(toEmails.SelectMany(x => x.BccEmails.Select(s => new EmailAddress(s.Email, s.Name))).ToList());

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
        private SendGridMessage GetAttachments(SendGridMessage msg, List<string> filePaths = null, FileResultDto fileResult = null)
        {
            if (filePaths != null && fileResult == null)
            {
                foreach (var filePath in filePaths)
                {
                    if (File.Exists(filePath))
                    {
                        msg.AddAttachment(new Attachment
                        {
                            Content = Convert.ToBase64String(File.ReadAllBytes(filePath)),
                            Filename = Path.GetFileName(filePath),
                            Type = GetMimeType(filePath),
                            Disposition = "attachment"
                        });
                    }
                }
            }

            if (fileResult != null && fileResult.FileBytes != null)
            {
                msg.AddAttachment(new Attachment
                {
                    Content = Convert.ToBase64String(fileResult.FileBytes),
                    Filename = fileResult.FileName,
                    Type = "application/octet-stream",
                    Disposition = "attachment"
                });
            }

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

        public async Task<bool> SendTemplatedEmailAsync(List<UserEmail> toEmails, string templateId, List<object> dynamicTemplateData, List<string> filePaths, FileResultDto fileResult = null)
        {
            return await SendEmailAsync(toEmails, templateId, dynamicTemplateData, filePaths, fileResult);
        }
        public async Task<bool> SendTextEmailAsync(List<UserEmail> toEmails, string subject, string message, List<string> filePaths)
        {
            try
            {
                var toMails = toEmails.Select(email => new EmailAddress(email.Email, email.Name)).ToList();
                var client = new SendGridClient(_mailSettings.Key);
                var from = new EmailAddress(_mailSettings.FromEmail, _mailSettings.DisplayName);
                var plainTextContent = "";
                var htmlContent = message;

                var subjects = toEmails.Select(e => subject).ToList();
                
                var substitutions = toEmails.Select(e => new Dictionary<string, string>
                {
                    { "-name-", e.Name }
                }).ToList();

                var msg = MailHelper.CreateMultipleEmailsToMultipleRecipients(from, toMails, subjects, plainTextContent, htmlContent, substitutions);

                if (filePaths != null && filePaths.Count > 0)
                    msg = GetAttachments(msg, filePaths);

                if (toEmails.Any(x => x.CCEmails?.Any() == true))
                {
                    var ccEmails = toEmails.SelectMany(x => x.CCEmails ?? new List<CCAndBccMailDto>())
                                           .Select(s => new EmailAddress(s.Email, s.Name))
                                           .ToList();
                    msg.AddCcs(ccEmails);
                }

                if (toEmails.Any(x => x.BccEmails?.Any() == true))
                {
                    var bccEmails = toEmails.SelectMany(x => x.BccEmails ?? new List<CCAndBccMailDto>())
                                            .Select(s => new EmailAddress(s.Email, s.Name))
                                            .ToList();
                    msg.AddBccs(bccEmails);
                }
                var response = await client.SendEmailAsync(msg);
                _logger.LogInformation("Message sent successfully.");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email.");
                return false;
            }
        }


        //public async Task<bool> SendTextEmailAsync(List<UserEmail> toEmails, string subject, string message, List<string> filePaths)
        //{
        //    try
        //    {
        //        var toMails = toEmails.Select(email => new EmailAddress(email.Email, email.Name)).ToList();
        //        var client = new SendGridClient(_mailSettings.Key);
        //        var from = new EmailAddress(_mailSettings.FromEmail, _mailSettings.DisplayName);
        //        var plainTextContent = "";
        //        var htmlContent = message;

        //        //var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, toMails, subject, plainTextContent, htmlContent);
        //        var msg = MailHelper.CreateMultipleEmailsToMultipleRecipients(from, toMails, subject,);


        //        if (filePaths != null && filePaths.Count > 0)
        //            msg = GetAttachments(msg, filePaths);

        //        if (toEmails.All(x => x.CCEmails.Any()))
        //            msg.AddCcs(toEmails.SelectMany(x => x.CCEmails.Select(s => new EmailAddress(s.Email, s.Name))).ToList());
        //        if (toEmails.All(x => x.BccEmails.Any()))
        //            msg.AddBccs(toEmails.SelectMany(x => x.BccEmails.Select(s => new EmailAddress(s.Email, s.Name))).ToList());

        //        var response = await client.SendEmailAsync(msg);
        //        _logger.LogInformation("Message sent successfully.");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogInformation(ex.Message);
        //        return false;
        //    }
        //}


    }
}

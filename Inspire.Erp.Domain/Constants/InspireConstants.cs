using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Constants
{
    public static class InspireConstants
    {
        public const string UserCacheKey = "LoggedinUser";

        public const string PurchaseRequisitionEmailTemplate = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <title>BOQ Update Notification</title>\r\n  <style>\r\n    body {\r\n      font-family: Arial, sans-serif;\r\n      background-color: #f9f9f9;\r\n      color: #333;\r\n      padding: 20px;\r\n    }\r\n    .email-container {\r\n      background-color: #ffffff;\r\n      padding: 20px;\r\n      border-radius: 6px;\r\n      box-shadow: 0 2px 4px rgba(0,0,0,0.1);\r\n      max-width: 600px;\r\n      margin: auto;\r\n    }\r\n    .header {\r\n      font-size: 18px;\r\n      font-weight: bold;\r\n      color: #2c3e50;\r\n      margin-bottom: 20px;\r\n    }\r\n    .content {\r\n      font-size: 16px;\r\n      line-height: 1.6;\r\n    }\r\n    .footer {\r\n      margin-top: 30px;\r\n      font-size: 14px;\r\n      color: #888;\r\n    }\r\n  </style>\r\n</head>\r\n<body>\r\n  <div class=\"email-container\">\r\n <div class=\"content\">\r\n      <p>Dear Recipient,</p>\r\n\r\n      <p>The Bill of Quantities (BOQ) for <strong>Job No. {{jobCode}} ({{jobName}})</strong> has increased from the original value of <strong>{{jobValue}}</strong> to <strong>{{prNetAmount}}</strong> following the revision of <strong>Purchase Requisition No. {{prNO}}</strong>.</p>\r\n\r\n      <p>You are requested to take appropriate action.</p>\r\n\r\n      <p>Thank you.</p>\r\n    </div>\r\n    <div class=\"footer\">\r\n      &copy; 2025 Water Bird Wtc. All rights reserved.\r\n    </div>\r\n  </div>\r\n</body>\r\n</html>\r\n";
       
        public const string BOQEmailTemplate = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <title>BOQ Update Notification</title>\r\n  <style>\r\n    body {\r\n      font-family: Arial, sans-serif;\r\n      background-color: #f9f9f9;\r\n      color: #333;\r\n      padding: 20px;\r\n    }\r\n    .email-container {\r\n      background-color: #ffffff;\r\n      padding: 20px;\r\n      border-radius: 6px;\r\n      box-shadow: 0 2px 4px rgba(0,0,0,0.1);\r\n      max-width: 600px;\r\n      margin: auto;\r\n    }\r\n    .header {\r\n      font-size: 18px;\r\n      font-weight: bold;\r\n      color: #2c3e50;\r\n      margin-bottom: 20px;\r\n    }\r\n    .content {\r\n      font-size: 16px;\r\n      line-height: 1.6;\r\n    }\r\n    .footer {\r\n      margin-top: 30px;\r\n      font-size: 14px;\r\n      color: #888;\r\n    }\r\n  </style>\r\n</head>\r\n<body>\r\n  <div class=\"email-container\">\r\n   <div class=\"content\">\r\n      <p>Dear Recipient,</p>\r\n\r\n      <p>The Bill of Quantities (BOQ) for <strong>Job No. {{jobCode}} ({{jobName}})</strong> has increased from the original value of <strong>{{jobValue}}</strong> to <strong>{{prNetAmount}}</strong> following the revision of <strong>BOQ No. {{prNO}}</strong>.</p>\r\n\r\n      <p>You are requested to take appropriate action.</p>\r\n\r\n      <p>Thank you.</p>\r\n    </div>\r\n    <div class=\"footer\">\r\n      &copy; 2025 Water Bird Wtc. All rights reserved.\r\n    </div>\r\n  </div>\r\n</body>\r\n</html>\r\n";

        public const string DefaultApprovalType = "DEFAULT_APPROVAL_TYPE";
        public const string ApprovalAsJobWise = "JOB_WISE::1";
        public const string ApprovalAsVoucherWise = "VOUCHER_WISE::2";
        public const string ApprovalAsNoApproval = "NO_APPROVAL::3";
    }
}

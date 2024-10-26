using Inspire.Erp.Domain.DTO;
using Inspire.Erp.Domain.Modals.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Account.Interfaces
{
    public interface IAvIssueVoucherService
    {
        public ResponseInfo GetDetailWiseReport(DTOIssueReportParam _DTOIssueReportParam);
        public IQueryable GetJobWiseReport(long? itemId, string partNo, long? departmentId, long? jobId, DateTime? dateFrom, DateTime? toDate,bool? isDate);
        public IQueryable GetSummaryWiseReport(long? itemId, string partNo, long? departmentId, long? jobId, DateTime? dateFrom, DateTime? toDate);
    }
}
using Inspire.Erp.Application.MIS.Implementations;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.MIS.Interfaces
{
    public interface IMisReportServices
    {
        Task<ApiResponse< SpProfitAndLossResponse>> GetProfitAndLossAsync(MisReportParams request);
        Task<ApiResponse<MisBalanceSheetResponse>> GetBalanceSheetAsync(MisReportParams request);
        Task<ApiResponse<MisTrialBalanceResponse>> GetTrialBalanceAsync(MisReportParams request);
    }
}

using Dapper;
using Inspire.Erp.Application.MIS.Interfaces;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Inspire.Erp.Application.MIS.Implementations;

public class MisReportServices : IMisReportServices
{
    private readonly IDbConnection _dbConnection;

    public MisReportServices(IOptions<ApplicationSettings> applicationSettings)
    {
        _dbConnection = new SqlConnection(applicationSettings.Value.ConnectionString);
    }

    public async Task<ApiResponse<MisBalanceSheetResponse>> GetBalanceSheetAsync(MisReportParams request)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FromDate", request.FromDate);
            parameters.Add("@ToDate", request.ToDate);
            parameters.Add("@IncludeZero", request.IncludeZero);
            parameters.Add("@IsSummaryView", request.IsSummaryView);

            var multi = await _dbConnection.QueryMultipleAsync("usp_GetBalanceSheet_Advanced", parameters, commandType: CommandType.StoredProcedure);
            var details = multi.Read<BalanceSheetDto>().ToList();
            var summary = multi.ReadSingleOrDefault<BalanceSheetSummaryDto>();

            return ApiResponse<MisBalanceSheetResponse>.Success(new MisBalanceSheetResponse
            {
                Details = details,
                Summary = summary
            }, "Data found");
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<ApiResponse<SpProfitAndLossResponse>> GetProfitAndLossAsync(MisReportParams request)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FromDate", request.FromDate);
            parameters.Add("@ToDate", request.ToDate);
            parameters.Add("@LocationId", request.LocationId);
            parameters.Add("@CostCenterId", request.CostCenterId);
            parameters.Add("@Mode", request.Mode);
            parameters.Add("@IsMonthly", request.IsMonthly);
            parameters.Add("@IsFullYear", request.IsFullYear);
            parameters.Add("@IncludeZero", request.IncludeZero);
            parameters.Add("@IsGroupWise", request.IsGroupWise);

            var multi = await _dbConnection.QueryMultipleAsync("usp_GetProfitAndLoss_Advanced", parameters, commandType: CommandType.StoredProcedure);
            var details = multi.Read<ProfitAndLossDto>().ToList();
            var summary = multi.ReadSingleOrDefault<ProfitAndLossSummaryDto>();

            return ApiResponse<SpProfitAndLossResponse>.Success(new SpProfitAndLossResponse
            {
                Details = details,
                Summary = summary
            }, "Data found");
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<ApiResponse<MisTrialBalanceResponse>> GetTrialBalanceAsync(MisReportParams request)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FromDate", request.FromDate);
            parameters.Add("@ToDate", request.ToDate);
            parameters.Add("@LocationId", request.LocationId);
            parameters.Add("@CostCenterId", request.CostCenterId);
            parameters.Add("@IncludeZero", request.IncludeZero);
            parameters.Add("@IsSummaryView", request.IsSummaryView);
            parameters.Add("@IsFullYear", request.IsFullYear);

            var multi = await _dbConnection.QueryMultipleAsync("usp_GetTrialBalance_Advanced", parameters, commandType: CommandType.StoredProcedure);
            var details = multi.Read<TrialBalanceDto>().ToList();
            var summary = multi.ReadSingleOrDefault<TrialBalanceSummaryDto>();

            return ApiResponse<MisTrialBalanceResponse>.Success(new MisTrialBalanceResponse
            {
                Details = details,
                Summary = summary
            }, "Data found");
        }
        catch (Exception)
        {

            throw;
        }
    }
}

#region Profit and loss
public class ProfitAndLossDto
{
    public string GroupKey { get; set; }
    public string HeadType { get; set; }
    public string SubHead { get; set; }
    public string AccountName { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public string MonthName { get; set; }
    public string MonthKey { get; set; }
}
public class ProfitAndLossSummaryDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalCOGS { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalOtherIncome { get; set; }
    public decimal TotalOtherExpense { get; set; }
    public decimal NetProfit { get; set; }
    public string FinalResult { get; set; }
}
public class SpProfitAndLossResponse
{
    public List<ProfitAndLossDto> Details { get; set; }
    public ProfitAndLossSummaryDto Summary { get; set; }
}
#endregion

#region Balance Sheet
public class MisBalanceSheetResponse
{
    public List<BalanceSheetDto> Details { get; set; }
    public BalanceSheetSummaryDto Summary { get; set; }
}


public class BalanceSheetDto
{
    public string HeadType { get; set; }
    public string SubHead { get; set; }
    public string AccountName { get; set; }
    public decimal NetAmount { get; set; }
    public decimal TotalAmount { get; set; }
}
public class BalanceSheetSummaryDto
{
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal TotalEquity { get; set; }
    public decimal LiabilitiesPlusEquity { get; set; }
    public decimal Difference { get; set; }
    public string FinalResult { get; set; }
}
#endregion

#region Trial balance


public class MisTrialBalanceResponse
{
    public List<TrialBalanceDto> Details { get; set; }
    public TrialBalanceSummaryDto Summary { get; set; }
}

public class TrialBalanceDto
{
    public string MainHead { get; set; }
    public string SubHead { get; set; }
    public string AccountName { get; set; }
    public decimal Opening { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Closing { get; set; }
}
public class TrialBalanceSummaryDto
{
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public decimal Difference { get; set; }
    public string FinalResult { get; set; }
}

#endregion

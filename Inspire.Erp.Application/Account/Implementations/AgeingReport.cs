using Dapper;
using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Application.Helpers.ExtensionMethods;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.AccountStatement;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Infrastructure;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Inspire.Erp.Application.Account.Implementations
{
    public class AgeingReport : IAgeingReport
    {
        private readonly IRepository<AccountTransacionsAgeingReport> _accountTransactions;
        private readonly IRepository<MasterAccountsTable> _MasterAccountsTable;
        private readonly IDbConnection _dbConnection;


        public AgeingReport(IRepository<AccountTransacionsAgeingReport> accountTransactions, IOptions<ApplicationSettings> applicationSettings, IRepository<MasterAccountsTable> masterAccountsTable)
        {
            _accountTransactions = accountTransactions;
            _MasterAccountsTable = masterAccountsTable;
            _dbConnection = new SqlConnection(applicationSettings.Value.ConnectionString);
        }
        public async Task<Response<AgeingReportResponse>> GetAgeingReport_II(GenericGridViewModel model)
        {
            try
            {
                AgeingReportResponse gridmodel = new AgeingReportResponse();
                string query = @$" AccountsTransactionsAllocBalance >0  {model.Filter}";
                var result = await _accountTransactions.GetBySPWithParameters<AccountTransacionsAgeingReport>(@$" exec GetAccountTransactionsAgeingReport {model.Skip},{model.Take},{model.Search},{model.Field},{model.Dir},{model.Filter},{model.Total}", x => new AccountTransacionsAgeingReport
                {
                    AccountsTransactionsAccNo = x.AccountsTransactionsAccNo,
                    AccountsTransactionsAccName = x.AccountsTransactionsAccName,
                    AccountsTransactionsDebit = Convert.ToDecimal(x.AccountsTransactionsDebit),
                    AccountsTransactionsCredit = Convert.ToDecimal(x.AccountsTransactionsCredit),
                    AccountsTransactionsAllocBalance = x.AccountsTransactionsAllocBalance,
                    //  AccountsTransactionsVoucherNo = x.AccountsTransactionsVoucherNo,
                    AccountsTransactionsVoucherType = x.AccountsTransactionsVoucherType,
                    AccountsTransactionsDescription = x.AccountsTransactionsDescription,
                    AccountsTransactionsTransDate = Convert.ToDateTime(x.AccountsTransactionsTransDate),
                    //  AccountsTransactionsParticulars = x.AccountsTransactionsParticulars,
                    RefNo = x.RefNo,
                    AccountsTransactionsTransSno = Convert.ToInt32(x.AccountsTransactionsTransSno),
                    AccountsTransactionsAllocDebit = x.AccountsTransactionsAllocDebit,
                    AccountsTransactionsAllocCredit = x.AccountsTransactionsAllocCredit
                });

                gridmodel.Details = new List<AgeingReportDetailsResponse>();
                foreach (var items in result.GroupBy(x => x.AccountsTransactionsAccNo))
                {
                    string accName = string.Empty;
                    string accNo = string.Empty;
                    decimal CrBal = 0;
                    decimal CrDys = 0;
                    decimal CrLimit = 0;
                    decimal UnAllocated = 0;
                    decimal outBalance = 0;
                    string tel1 = string.Empty;
                    string Contact_Person = string.Empty;
                    string CT_Name = string.Empty;
                    string Mobile = string.Empty;
                    string Remarks = string.Empty;
                    decimal runningBalance = 0;
                    decimal ZeroToThirty = 0;
                    decimal ThirtyOneToSixty = 0;
                    decimal SixtyOneToNinety = 0;
                    decimal NinetyOneToOneEighty = 0;
                    decimal OneEightOneToTwoSeventy = 0;
                    decimal TwoSeventyOneToThreeSixty = 0;
                    decimal aboveThreeSixty = 0;
                    foreach (var accounts in items)
                    {
                        CrBal += Convert.ToDecimal(accounts.AccountsTransactionsAllocBalance);
                        CrLimit += 0;
                        accName = accounts.AccountsTransactionsAccName;
                        accNo = accounts.AccountsTransactionsAccNo;
                        Remarks = accounts.AccountsTransactionsDescription;

                        if (accounts == null || accounts.AccountsTransactionsAccNo == null)
                        {
                            // Handle the null case as appropriate
                            return null; // Or throw an exception or whatever logic you need
                        }
                        var account = await _MasterAccountsTable.GetAsQueryable()
                            .Where(c => c.MaAccNo == accounts.AccountsTransactionsAccNo)
                            .FirstOrDefaultAsync();
                        if (account.MaAccName == "Supplier")
                        {

                            UnAllocated -= accounts.AccountsTransactionsAllocBalance.Value;
                        }

                        else if (accounts.AccountsTransactionsCredit > 0 && accounts.AccountsTransactionsAllocBalance > 0)
                        {
                            UnAllocated += accounts.AccountsTransactionsAllocBalance.Value;
                        }

                        if (account.MaAccName == "Supplier")
                        {

                            outBalance += accounts.AccountsTransactionsCredit.Value - accounts.AccountsTransactionsDebit.Value;
                        }
                        else
                        {
                            outBalance += accounts.AccountsTransactionsDebit.Value - accounts.AccountsTransactionsCredit.Value;

                        }


                        //decimal totalDebit = accounts.AccountsTransactionsDebit;
                        //decimal totalCredit = accounts.AccountsTransactionsCredit;
                        //runningBalance = (totalDebit - totalCredit) + (runningBalance);
                        if (accounts.AccountsTransactionsDebit > 0)
                        {
                            var days = Math.Round(((DateTime.Now - Convert.ToDateTime(accounts.AccountsTransactionsTransDate)).TotalDays));
                            CrDys += Convert.ToDecimal(days);

                            if (days >= 0 && days < 31)
                            {
                                ZeroToThirty += Convert.ToDecimal(accounts.AccountsTransactionsAllocBalance);
                            }
                            else if (days >= 31 && days < 60)
                            {
                                ThirtyOneToSixty += Convert.ToDecimal(accounts.AccountsTransactionsAllocBalance);
                            }
                            else if (days >= 61 && days < 90)
                            {
                                SixtyOneToNinety += Convert.ToDecimal(accounts.AccountsTransactionsAllocBalance);
                            }
                            else if (days >= 91 && days < 180)
                            {
                                NinetyOneToOneEighty += Convert.ToDecimal(accounts.AccountsTransactionsAllocBalance);
                            }
                            else if (days >= 181 && days < 270)
                            {
                                OneEightOneToTwoSeventy += Convert.ToDecimal(accounts.AccountsTransactionsAllocBalance);
                            }
                            else if (days >= 271 && days < 360)
                            {
                                TwoSeventyOneToThreeSixty += Convert.ToDecimal(accounts.AccountsTransactionsAllocBalance);
                            }
                            else if (days > 360)
                            {
                                aboveThreeSixty += Convert.ToDecimal(accounts.AccountsTransactionsAllocBalance);
                            }
                        }
                    }
                    gridmodel.Details.Add(new AgeingReportDetailsResponse()
                    {
                        CrBal = CrBal.ToString(),
                        CrDys = CrDys.ToString(),
                        CrLimit = CrLimit.ToString(),
                        Name = accName,
                        AccNo = accNo,
                        UnAllocated = UnAllocated.ToString(),
                        Contact_Person = Contact_Person,
                        CT_Name = CT_Name,
                        Mobile = Mobile,
                        NinetyOneToOneEighty = NinetyOneToOneEighty.ToString(),
                        OneEightOneToTwoSeventy = OneEightOneToTwoSeventy.ToString(),
                        OutBalance = outBalance.ToString(),
                        OverThreeSixty = aboveThreeSixty.ToString(),
                        Remarks = Remarks,
                        SixtyOneNinety = SixtyOneToNinety.ToString(),
                        Tel1 = tel1,
                        ThirtyOneSixty = ThirtyOneToSixty.ToString(),
                        TwoSeventyOneToThreeSixty = TwoSeventyOneToThreeSixty.ToString(),
                        ZeroThirty = ZeroToThirty.ToString(),
                    });

                }
                return Response<AgeingReportResponse>.Success(gridmodel, "RecordFound");
            }
            catch (Exception ex)
            {
                return Response<AgeingReportResponse>.Fail(new AgeingReportResponse(), ex.Message);
            }
        }

        public async Task<Response<AgeingReportResponse>> GetAgeingReport(GenericGridViewModel model)
        {
            try
            {
                // Build SQL EXEC command safely
                string sql = $@"
                                            DECLARE @Total INT;

                                            EXEC usp_GetAgeingReport_Advanced 
                                                @AsOnDate = '{model.ToDate:yyyy-MM-dd}', 
                                                @IncludeAllocBalance = 1, 
                                                @AllocBalanceMin = 0,
                                                @RelativeNo = {model.Filter}, 
                                                @Search = '{model.Search?.Replace("'", "''") ?? ""}', 
                                                @Skip = {model.Skip}, 
                                                @Take = {model.Take}, 
                                                @SortField = 'Name', 
                                                @SortDir = 'ASC', 
                                                @ShowUnallocated = {model.ShowUnallocated}, 
                                                @TotalCount = @Total OUTPUT;

                                            SELECT @Total AS TotalCount;";

                // --- Execute ---
                using var multi = await _dbConnection.QueryMultipleAsync(sql);

                // 1️⃣ Read main dataset
                var data = (await multi.ReadAsync<dynamic>()).ToList();

                // 2️⃣ Read total count
                int totalCount = (await multi.ReadFirstOrDefaultAsync<int>());

                // --- Map records ---
                var details = data.Select(x =>
                {
                    var dict = (IDictionary<string, object>)x;

                    return new AgeingReportDetailsResponse
                    {
                        Name = dict.GetValueOrDefault("Name")?.ToString() ?? "",
                        AccNo = dict.GetValueOrDefault("accNo")?.ToString() ?? "",
                        CrLimit = Convert.ToDecimal(dict.GetValueOrDefault("CrLimit") ?? 0).ToString("N2"),
                        CrDys = Convert.ToInt32(dict.GetValueOrDefault("CrDys") ?? 0).ToString(),

                        ZeroThirty = Convert.ToDecimal(dict.GetValueOrDefault("0_30") ?? 0).ToString("N2"),
                        ThirtyOneSixty = Convert.ToDecimal(dict.GetValueOrDefault("31_60") ?? 0).ToString("N2"),
                        SixtyOneNinety = Convert.ToDecimal(dict.GetValueOrDefault("61_90") ?? 0).ToString("N2"),
                        NinetyOneToOneEighty = Convert.ToDecimal(dict.GetValueOrDefault("91_180") ?? 0).ToString("N2"),
                        OneEightOneToTwoSeventy = Convert.ToDecimal(dict.GetValueOrDefault("181_270") ?? 0).ToString("N2"),
                        TwoSevenOneToThreeSixty = Convert.ToDecimal(dict.GetValueOrDefault("271_360") ?? 0).ToString("N2"),
                        OverThreeSixty = Convert.ToDecimal(dict.GetValueOrDefault("OV360") ?? 0).ToString("N2"),

                        CrBal = Convert.ToDecimal(dict.GetValueOrDefault("CrBal") ?? 0).ToString("N2"),
                        UnAllocated = Convert.ToDecimal(dict.GetValueOrDefault("UnAllocated") ?? 0).ToString("N2"),
                        OutBalance = Convert.ToDecimal(dict.GetValueOrDefault("outBalance") ?? 0).ToString("N2"),

                        Contact_Person = dict.GetValueOrDefault("contact_Person")?.ToString() ?? "",
                        CT_Name = dict.GetValueOrDefault("cT_Name")?.ToString() ?? "",
                        Tel1 = dict.GetValueOrDefault("tel1")?.ToString() ?? "",
                        Mobile = dict.GetValueOrDefault("mobile")?.ToString() ?? "",
                        Remarks = dict.GetValueOrDefault("remarks")?.ToString() ?? ""
                    };
                }).ToList();

                // --- Totals ---
                var response = new AgeingReportResponse
                {
                    Details = details,
                    CreditBalance = details.Sum(x => Convert.ToDecimal(x.CrBal)).ToString("N2"),
                    Total = details.Sum(x => Convert.ToDecimal(x.OutBalance)).ToString("N2")
                };

                return Response<AgeingReportResponse>.Success(response, "Ageing report generated successfully", totalCount);
            }
            catch (Exception ex)
            {
                return Response<AgeingReportResponse>.Fail(new AgeingReportResponse(), ex.Message);
            }
        }

    }
}

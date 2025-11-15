using Inspire.Erp.Application.Sales.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals.Sales;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Sales.Implementation
{
    public class SalesmanwiseSalesReport: ISalesmanwiseSalesReport
    {
        private readonly IRepository<ItemMaster> _itemMaster;
        private readonly IRepository<StationMaster> _stationMaster;
        private readonly IRepository<AccountsTransactions> _accountTransaction;
        private readonly IRepository<CustomerMaster> _customerMaster;
        private readonly IRepository<SalesManMaster> _salesmanMaster;
        public SalesmanwiseSalesReport(IRepository<ItemMaster> itemMaster, IRepository<StationMaster> stationMaster,
            IRepository<AccountsTransactions> accountTransaction, IRepository<CustomerMaster> customerMaster, IRepository<SalesManMaster> salesmanMaster)
        {
            _itemMaster = itemMaster;
            _accountTransaction = accountTransaction;
            _stationMaster = stationMaster;
            _salesmanMaster = salesmanMaster;
            _customerMaster = customerMaster;
        }
        public async Task<Response<List<DropdownResponse>>> GetSalesManMaster()
        {
            try
            {
                var result = await _salesmanMaster.GetAsQueryable().ToListAsync();
                var response = new List<DropdownResponse>();
                //response.Add(new DropdownResponse()
                //{
                //    Value = "",
                //    Name = "All"
                //});
                response.AddRange(result.Select(x => new DropdownResponse { 
                Id=x.SalesManMasterSalesManId != null? Convert.ToInt32(x.SalesManMasterSalesManId):0,
                Name=x.SalesManMasterSalesManName
                }).ToList());
                response.AddRange(response.OrderBy(x => x.Name)); // Order by Name
                return Response<List<DropdownResponse>>.Success(response, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<DropdownResponse>>.Fail(new List<DropdownResponse>(), ex.Message);
            }
        }
        public async Task<Response<List<DropdownResponse>>> GetCustomerMasterDropdownFromSalesman(string query)
        {
            try
            {
                var result = await _customerMaster.GetAsQueryable().Where(query).ToListAsync();
                var response = new List<DropdownResponse>();
                //string allSearch = "";
                //foreach (var item in result)
                //{
                //    allSearch += @$" (AccountsTransactionsAccNo== {item.CustomerMasterCustomerReffAcNo}) ||";
                //}
                //response.Add(new DropdownResponse()
                //{
                //    Value = "",
                //    Name = "All"
                //});
                response.AddRange(result.Select(x => new DropdownResponse
                {
                    Name=x.CustomerMasterCustomerName,
                    Value=x.CustomerMasterCustomerReffAcNo
                }).ToList());
                response.AddRange(response.OrderBy(x => x.Name)); // Order by Name
                return Response<List<DropdownResponse>>.Success(response, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<DropdownResponse>>.Fail(new List<DropdownResponse>(), ex.Message);
            }
        }
        public async Task<Response<GetSalesmanOutstandingReportResponse>> AccountTransactionsSalesmanWiseOutstandingReportDetail(GenericGridViewModel model)
        {
            try
            {
                GetSalesmanOutstandingReportResponse gridModel = new GetSalesmanOutstandingReportResponse();
                gridModel.Details = new List<GetSalesmanOutstandingReportDetailsResponse>();
                string query = @$" 1==1 {model.Filter}";
                var location = await _stationMaster.GetAsQueryable().FirstOrDefaultAsync();
                //var a = await _accountTransaction.GetAsQueryable().Where(x =>( x.AccountsTransactionsTransDate >= Convert.ToDateTime("")) &&  )
                //    .ToListAsync();
                var result = await _accountTransaction.GetAsQueryable().Where(query).Select(x => new GetSalesmanOutstandingReportDetailsResponse
                {
                    AccountsTransactionsAccNo = x.AccountsTransactionsAccNo,
                  //  AccountsTransactionsAccName = _masterAccountsTable.GetAsQueryable().FirstOrDefault(c => c.MasterAccountsTableAccNo == x.AccountsTransactionsAccNo) != null ? _masterAccountsTable.GetAsQueryable().FirstOrDefault(c => c.MasterAccountsTableAccNo == x.AccountsTransactionsAccNo).MasterAccountsTableAccName : "",
                    AccountsTransactionsDebit = Convert.ToDecimal(x.AccountsTransactionsDebit),
                    AccountsTransactionsCredit = Convert.ToDecimal(x.AccountsTransactionsCredit),
                    AccountsTransactionsAllocBalance = x.AccountsTransactionsAllocBalance,
                    AccountsTransactionsVoucherNo = x.AccountsTransactionsVoucherNo,
                    AccountsTransactionsVoucherType = x.AccountsTransactionsVoucherType,
                    AccountsTransactionsDescription = x.AccountsTransactionsDescription,
                    AccountsTransactionsTransDate = Convert.ToDateTime(x.AccountsTransactionsTransDate),
                    AccountsTransactionsTransDateString = Convert.ToDateTime(x.AccountsTransactionsTransDate).ToString("MM-dd-yyyy"),
                    AccountsTransactionsParticulars = x.AccountsTransactionsParticulars,
                    RefNo = x.RefNo,
                    AccountsTransactionsTransSno = Convert.ToInt32(x.AccountsTransactionsTransSno),
                    AccountsTransactionsAllocDebit = x.AccountsTransactionsAllocDebit,
                    AccountsTransactionsAllocCredit = x.AccountsTransactionsAllocCredit
                }).ToListAsync();
                decimal totalDebit = 0;
                decimal totalCredit = 0;
                decimal totalSettled = 0;
                decimal totalRunningBalance = 0;
                foreach (var item in result)
                {
                     totalDebit += item.AccountsTransactionsDebit;
                     totalCredit += item.AccountsTransactionsCredit;
                    totalSettled += item.AccountsTransactionsCredit;
                    totalRunningBalance = (item.AccountsTransactionsDebit - item.AccountsTransactionsCredit) + (totalRunningBalance);
                    gridModel.Details.Add(new GetSalesmanOutstandingReportDetailsResponse()
                    {
                        AccountsTransactionsAccName = _customerMaster.GetAsQueryable().FirstOrDefault(x => x.CustomerMasterCustomerReffAcNo == item.AccountsTransactionsAccNo) != null ? _customerMaster.GetAsQueryable().FirstOrDefault(x => x.CustomerMasterCustomerReffAcNo == item.AccountsTransactionsAccNo).CustomerMasterCustomerName : "",
                        AccountsTransactionsDebit = item.AccountsTransactionsDebit,
                        AccountsTransactionsCredit = item.AccountsTransactionsCredit,
                        AccountsTransactionsAccNo = item.AccountsTransactionsAccNo,
                        SalesManName = model.Field,
                        AccountsTransactionsVoucherNo = item.AccountsTransactionsVoucherNo,
                        AccountsTransactionsVoucherType = item.AccountsTransactionsVoucherType,
                        AccountsTransactionsTransDateString = item.AccountsTransactionsTransDateString,
                        AccountsTransactionsDescription=item.AccountsTransactionsDescription,
                        Settled=item.AccountsTransactionsDebit,
                        RunningBalance= totalRunningBalance
                    }) ; 
                }
                gridModel.StationMasterStationName = location != null ? location.StationMasterStationName : "";
                gridModel.StationMasterEmail = location != null ? location.StationMasterEmail : "";
                gridModel.StationMasterFax = location != null ? location.StationMasterFax : "";
                gridModel.StationMasterTele1 = location != null ? location.StationMasterTele1 : "";
                gridModel.StationMasterAddress = location != null ? location.StationMasterAddress : "";
                gridModel.StationMasterCity = location != null ? location.StationMasterCity : "";
                gridModel.StationMasterCode = location != null ? location.StationMasterCode : 0;
                gridModel.StationMasterCountry = location != null ? location.StationMasterCountry : "";
                gridModel.StationMasterLogoPath = location != null ? location.StationMasterLogoPath : "";
                gridModel.StationMasterPostOffice = location != null ? location.StationMasterPostOffice : "";
                gridModel.StationMasterSignPath = location != null ? location.StationMasterSignPath : "";
                gridModel.StationMasterVatNo = location != null ? location.StationMasterVatNo : "";
                gridModel.StationMasterWebSite = location != null ? location.StationMasterWebSite : "";
                gridModel.TotalSettled = totalSettled.ToString();
                gridModel.TotalCredit = totalCredit.ToString();
                gridModel.TotalDebit = totalDebit.ToString();
                gridModel.TotalRunningBalance = totalRunningBalance.ToString();
                return Response<GetSalesmanOutstandingReportResponse>.Success(gridModel, "Data found");
            }
            catch (Exception ex)
            {
                return Response<GetSalesmanOutstandingReportResponse>.Fail(new GetSalesmanOutstandingReportResponse(), ex.Message);
            }
        }
        public async Task<Response<GetSalesmanOutstandingReportResponse>> AccountTransactionsSalesmanWiseOutstandingReportSummary(GenericGridViewModel model)
        {
            try
            {
                // Initialize response
                var gridModel = new GetSalesmanOutstandingReportResponse
                {
                    Details = new List<GetSalesmanOutstandingReportDetailsResponse>()
                };

                // Safe dynamic filter
                string query = "1 == 1";
                if (!string.IsNullOrWhiteSpace(model.Filter))
                {
                    query += " AND (" + model.Filter + ")";
                }

                // Get station info once
                var location = await _stationMaster.GetAsQueryable().FirstOrDefaultAsync();
                if (location != null)
                {
                    gridModel.StationMasterStationName = location.StationMasterStationName;
                    gridModel.StationMasterEmail = location.StationMasterEmail;
                    gridModel.StationMasterFax = location.StationMasterFax;
                    gridModel.StationMasterTele1 = location.StationMasterTele1;
                    gridModel.StationMasterAddress = location.StationMasterAddress;
                    gridModel.StationMasterCity = location.StationMasterCity;
                    gridModel.StationMasterCode = location.StationMasterCode;
                    gridModel.StationMasterCountry = location.StationMasterCountry;
                    gridModel.StationMasterLogoPath = location.StationMasterLogoPath;
                    gridModel.StationMasterPostOffice = location.StationMasterPostOffice;
                    gridModel.StationMasterSignPath = location.StationMasterSignPath;
                    gridModel.StationMasterVatNo = location.StationMasterVatNo;
                    gridModel.StationMasterWebSite = location.StationMasterWebSite;
                }

                // Fetch transactions with customer info in a single query
                var txnQuery = _accountTransaction.GetAsQueryable()
                    .Where(query)
                    .GroupJoin(
                        _customerMaster.GetAsQueryable(),
                        txn => txn.AccountsTransactionsAccNo,
                        cust => cust.CustomerMasterCustomerReffAcNo,
                        (txn, custGroup) => new { txn, custGroup }
                    )
                    .SelectMany(
                        x => x.custGroup.DefaultIfEmpty(),
                        (x, cust) => new GetSalesmanOutstandingReportDetailsResponse
                        {
                            AccountsTransactionsAccNo = x.txn.AccountsTransactionsAccNo,
                            AccountsTransactionsAccName = cust != null ? cust.CustomerMasterCustomerName : "",
                            AccountsTransactionsDebit = x.txn.AccountsTransactionsDebit,
                            AccountsTransactionsCredit = x.txn.AccountsTransactionsCredit,
                            AccountsTransactionsTransDateString = x.txn.AccountsTransactionsTransDate.ToString(),
                            AccountsTransactionsVoucherNo = x.txn.AccountsTransactionsVoucherNo,
                            AccountsTransactionsVoucherType = x.txn.AccountsTransactionsVoucherType,
                            AccountsTransactionsDescription = x.txn.AccountsTransactionsDescription,
                            SalesManName = model.Field
                        }
                    );

                var result = await txnQuery.ToListAsync();

                // Totals calculation & grouping
                decimal totalDebit = 0;
                decimal totalCredit = 0;
                decimal totalSettled = 0;
                decimal totalRunningBalance = 0;

                var grouped = result.GroupBy(x => x.AccountsTransactionsAccNo);

                foreach (var group in grouped)
                {
                    decimal runningBalance = 0;

                    foreach (var item in group)
                    {
                        runningBalance += item.AccountsTransactionsDebit - item.AccountsTransactionsCredit;
                        totalDebit += item.AccountsTransactionsDebit;
                        totalCredit += item.AccountsTransactionsCredit;
                        totalSettled += item.AccountsTransactionsCredit;

                        item.RunningBalance = runningBalance;
                        item.Settled = item.AccountsTransactionsDebit; // Adjust if Settled logic differs

                        gridModel.Details.Add(item);
                    }

                    // Optional summary per account
                    gridModel.Details.Add(new GetSalesmanOutstandingReportDetailsResponse
                    {
                        AccountsTransactionsAccNo = group.Key,
                        AccountsTransactionsAccName = group.First().AccountsTransactionsAccName,
                        AccountsTransactionsDebit = group.Sum(x => x.AccountsTransactionsDebit),
                        AccountsTransactionsCredit = group.Sum(x => x.AccountsTransactionsCredit),
                        Settled = group.Sum(x => x.AccountsTransactionsDebit),
                        RunningBalance = group.Sum(x => x.AccountsTransactionsDebit - x.AccountsTransactionsCredit),
                        SalesManName = model.Field
                    });
                }

                // Totals
                gridModel.TotalDebit = totalDebit.ToString();
                gridModel.TotalCredit = totalCredit.ToString();
                gridModel.TotalSettled = totalSettled.ToString();
                gridModel.TotalRunningBalance = totalRunningBalance.ToString();

                return Response<GetSalesmanOutstandingReportResponse>.Success(gridModel, "Data found");
            }
            catch (Exception ex)
            {
                return Response<GetSalesmanOutstandingReportResponse>.Fail(new GetSalesmanOutstandingReportResponse(), ex.Message);
            }
        }

    }
}
